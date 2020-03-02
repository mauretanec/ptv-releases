/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    internal class WebPageCleanerJob : BaseJob
    {
        private const int PageSize = 500;
        
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            if (!DuplicatesExist(contextManager))
            {
                TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, "No duplicate web pages were found. Cancelling the job.");
                return "No duplicate web pages were found. Cancelling the job.";
            }
            
            var uniquePages = GetUniquePages(contextManager);

            ProcessEntities<ServiceWebPage>(contextManager, uniquePages);
            ProcessEntities<OrganizationWebPage>(contextManager, uniquePages);
            ProcessEntities<ServiceServiceChannelWebPage>(contextManager, uniquePages);
            ProcessEntities<LawWebPage>(contextManager, uniquePages);
            ProcessEntities<ServiceChannelWebPage>(contextManager, uniquePages);

            ProcessUrls<ElectronicChannelUrl>(contextManager, uniquePages, ec => ec.ElectronicChannelId);
            ProcessUrls<WebpageChannelUrl>(contextManager, uniquePages, wc => wc.WebpageChannelId);
            ProcessUrls<PrintableFormChannelUrl>(contextManager, uniquePages, pc => pc.PrintableFormChannelId);
            
            CleanUnusedWebPages(uniquePages.Values.ToList(), contextManager);
            TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, $"{DateTime.UtcNow} Finished.");
            return null;
        }

        private bool DuplicatesExist(IContextManager contextManager)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
                var urlGroups = webPageRepo.All().GroupBy(x => x.Url);
                return urlGroups.Select(g => new {g.Key, Count = g.Count()}).Any(x => x.Count > 1);
            });
        }

        private Dictionary<string, Guid> GetUniquePages(IContextManager contextManager)
        {
            TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, $"{DateTime.UtcNow} Loading unique web pages...");
            var entities = contextManager.ExecuteReader(unitOfWork =>
            {
                var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
                return webPageRepo.All()
                    .Select(x => new {x.Url, x.Id})
                    .ToList();
            });
            var uniquePages = entities
                .Select(x => new {FormattedUrl = (x.Url ?? "").Trim(), Id = x.Id})
                .GroupBy(x => x.FormattedUrl)
                .ToDictionary(x => x.Key, x => x.First().Id);
            return uniquePages;
        }

        private void ProcessEntities<T>(IContextManager contextManager, IReadOnlyDictionary<string, Guid> uniquePages)
            where T : class, IEntityWithWebPage, IEntityIdentifier
        {
            var entityCount = contextManager.ExecuteReader(unitOfWork =>
            {
                var entityRepo = unitOfWork.CreateRepository<IRepository<T>>();
                return entityRepo.All().Count();
            });

            TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey,
                $"{DateTime.UtcNow} Cleaning {entityCount} duplicate web pages of type {typeof(T).Name}...");

            for (var index = 0; index < entityCount; index += PageSize)
            {
                TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, $"Web page {index}/{entityCount}");
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var entityRepo = unitOfWork.CreateRepository<IRepository<T>>();
                    var entityWebPages = entityRepo.All()
                        .Include(x => x.WebPage)
                        .OrderBy(x => x.Id)
                        .Skip(index)
                        .Take(PageSize)
                        .ToList();

                    foreach (var entity in entityWebPages)
                    {
                        var formattedUrl = (entity.WebPage?.Url ?? "").Trim();
                        var uniqueId = uniquePages[formattedUrl];

                        if (entity.WebPageId != uniqueId)
                        {
                            entity.WebPageId = uniqueId;
                        }
                    }

                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }

        private void ProcessUrls<T>(IContextManager contextManager, Dictionary<string, Guid> uniquePages, Expression<Func<T, Guid>> orderBy)
            where T : class, IUrl, IEntityWithWebPage
        {
            var entityCount = contextManager.ExecuteReader(unitOfWork =>
            {
                var entityRepo = unitOfWork.CreateRepository<IRepository<T>>();
                return entityRepo.All().Count();
            });

            TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey,
                $"{DateTime.UtcNow} Processing {entityCount} urls of type {typeof(T).Name}...");

            for (var index = 0; index < entityCount; index += PageSize)
            {
                TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, $"Web page {index}/{entityCount}");

                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var entityRepo = unitOfWork.CreateRepository<IRepository<T>>();
                    var entities = entityRepo.All()
                        .OrderBy(orderBy)
                        .Skip(index)
                        .Take(PageSize)
                        .ToList();

                    foreach (var entity in entities)
                    {
                        var formattedUrl = (entity.Url ?? "").Trim();

                        if (uniquePages.TryGetValue(formattedUrl, out var uniqueId))
                        {
                            entity.WebPageId = uniqueId;
                        }
                        else
                        {
                            var newWebPage = CreateWebPage(formattedUrl, unitOfWork);
                            unitOfWork.Save(SaveMode.AllowAnonymous);

                            if (!(newWebPage?.Id).IsAssigned())
                            {
                                throw new Exception($"Web page with url {formattedUrl} could not be added to the DB.");
                            }

                            entity.WebPageId = newWebPage.Id;
                            uniquePages.Add(formattedUrl, newWebPage.Id);
                        }
                    }

                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }
        
        private WebPage CreateWebPage(string formattedUrl, IUnitOfWork unitOfWork)
        {
            var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
            var newWebPage = new WebPage {Url = formattedUrl};
            webPageRepo.Add(newWebPage);
            return newWebPage;
        }

        private void CleanUnusedWebPages(List<Guid> uniquePageIds, IContextManager contextManager)
        {
            TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, $"{DateTime.UtcNow} Deleting duplicates...");

            var resultsCount = 100;
            var deletionsCount = 0;
            do
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
                    var batchToDelete = webPageRepo.All()
                        .Where(x => !uniquePageIds.Contains(x.Id)).Take(500).ToList();
                    resultsCount = batchToDelete.Count;
                    deletionsCount += resultsCount;
                    
                    webPageRepo.Remove(batchToDelete);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    TaskSchedulerLogger.LogJobInfo(JobOperationId, JobKey, $"{DateTime.UtcNow} Deleted {deletionsCount} web pages.");
                });
            } while (resultsCount > 0);
        }
    }
}
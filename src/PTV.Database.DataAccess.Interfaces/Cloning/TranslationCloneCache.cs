﻿/**
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Cloning
{
    [RegisterService(typeof(ITranslationCloneCache), RegisterType.Transient)]
    public class TranslationCloneCache : ITranslationCloneCache
    {
        private readonly Dictionary<Type, List<IClonedTraceIdentification>> cache = new Dictionary<Type, List<IClonedTraceIdentification>>();
        private readonly Dictionary<Type, EntityPropertiesDefinition> entityNavigationsMap;

        public TranslationCloneCache(IEntityNavigationsMap entityNavigationsMap)
        {
            this.entityNavigationsMap = entityNavigationsMap?.NavigationsMap;
        }

        public void AddToCachedSet<T>(T entityOriginal, T entityCloned) where T : class
        {
            List<IClonedTraceIdentification> entityStorage = cache.TryGet(typeof(T));
            if (entityStorage == null)
            {
                entityStorage = new List<IClonedTraceIdentification>();
                cache[typeof(T)] = entityStorage;
            }
            entityStorage.Add(new ClonedTraceIdentification<T>() { OriginalEntity = entityOriginal, ClonedEntity = entityCloned});
        }

        public List<IClonedTraceIdentification<T>> GetFromCachedSet<T>() where T : class
        {
            var data = (cache.TryGet(typeof(T)) ?? new List<IClonedTraceIdentification>()).Cast<IClonedTraceIdentification<T>>();
            return data.ToList();
        }

        private void GetChildrenSubCascade(Dictionary<Type, List<IClonedTraceIdentification>> perTypeCacheData, IList<object> rootEntites, ref List<object> result)
        {
            var rowType = rootEntites.FirstOrDefault()?.GetType();
            if (rowType == null) return;
            foreach (var nm in entityNavigationsMap[rowType].NavigationsMap)
            {
                var relevantEntities = perTypeCacheData.TryGet(nm.Navigation.Type)?.Select(i => i.ClonedEntity)?.ToList() ?? new List<object>();
                relevantEntities = rootEntites.Select(i => i.GetPropertyObjectValue(nm.Navigation.Name)).Intersect(relevantEntities).Except(result).ToList();
                if (relevantEntities.Any())
                {
                    result.AddRange(relevantEntities);
                    GetChildrenSubCascade(perTypeCacheData, relevantEntities, ref result);
                }
            }
            foreach (var entityType in entityNavigationsMap.Values.Where(i => i.NavigationsMap.Any(u => u.Navigation.Type == rowType)))
            {
                var propertyName = entityType.NavigationsMap.First(j => j.Navigation.Type == rowType).Navigation.Name;
                var relevantEntities = perTypeCacheData.TryGet(entityType.EntityType)?.Select(j => j.ClonedEntity)?.ToList() ?? new List<object>();
                var newSubEntities = relevantEntities.Select(i => new {Related = i.GetPropertyObjectValue(propertyName), Main = i}).Where(i => rootEntites.Any(j => j == i.Related)).Select(j => j.Main).Except(result).ToList();
                if (newSubEntities.Any())
                {
                    result.AddRange(newSubEntities);
                    GetChildrenSubCascade(perTypeCacheData, newSubEntities, ref result);
                }
            }
        }

        public IList<object> GetEntitiesCascade(IList<object> rootEntites)
        {
            if (entityNavigationsMap == null) return new List<object>();
            var allCachedData = cache.Values.SelectMany(j => j).Where(j => !j.ProcessedByTranslator).ToList();
            var perTypeCacheData = allCachedData.Select(i => new { EType = i.OriginalEntity.GetType(), Obj = i }).GroupBy(i => i.EType).ToDictionary(i => i.Key, i => i.Select(j => j.Obj).ToList());
            var result = new List<object>();
            GetChildrenSubCascade(perTypeCacheData, rootEntites, ref result);
            return result;
        }

        public void MarkAsProcessedByTranslator<T>(T clonedEntity) where T : class
        {
            var cachedInfo = this.GetFromCachedSet<T>().FirstOrDefault(i => i.ClonedEntity == clonedEntity);
            cachedInfo.SafeCall(i => i.ProcessedByTranslator = true);
        }
    }
}

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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq;

using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.ApplicationDbContext;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PTV.Framework;

namespace PTV.Database.DataAccess.Repositories
{
    /// <summary>
    /// Base of base repository interface with basic set of methods.
    /// </summary>
    internal class Repository : IRepository
    {
    	private PtvDbContext ptvDbContext;
    
    	/// <summary>
        /// DataContext which is used by this repository instance.
        /// </summary>
        protected PtvDbContext DataContext
        {
            get
            {
                return ptvDbContext;
            }
            set
            {
                ptvDbContext = value;
            }
        }
    
    	#region Ctor
    
        /// <summary>
        /// Default constructor for base repository.
        /// </summary>
        /// <exception cref="ArgumentNullException">When context is null.</exception>
        /// <param name="context">Data context for repository instance.</param>
        public Repository(PtvDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Argument 'context' can not be null!");
            }
    
            DataContext = context;
        }
    
        #endregion Ctor
    }
    
    /// <summary>
    /// Base repository implementation.
    /// </summary>
    /// <typeparam name="TEntity">Model class type.</typeparam>
    internal class Repository<TEntity> : Repository, IRepository<TEntity> where TEntity : class, new()
    {
        private DbSet<TEntity> dbSet;
    	private IPrefilteringManager prefilteringManager;
    
        /// <summary>
        /// Collection all items if type <code>TEntity</code>. It is used for add and remove items.
        /// </summary>
        private DbSet<TEntity> DbSet
        {
            get
            {
                return dbSet ?? (dbSet = DataContext.Set<TEntity>());
            }
        }
    
        #region Ctor
    
        /// <summary>
        /// Default constructor for base repository.
        /// </summary>
        /// <exception cref="ArgumentNullException">When context is null.</exception>
        /// <param name="context">Data context for repository instance.</param>
        /// <param name="prefilteringManager">Prefiltering manager for query filters.</param>
        public Repository(PtvDbContext context, IPrefilteringManager prefilteringManager)
        		: base(context)
        {
             this.prefilteringManager = prefilteringManager;
        }
    
        #endregion Ctor
    
        public void BatchInsert(IEnumerable<TEntity> entities, string userName)
        {
            DataContext.BatchInsert(entities, userName);
        }
    
        public void BatchUpdate<T2>(IEnumerable<TEntity> entities, Expression<Func<TEntity, T2>> memberLamda, string userName)
        {
        
            DataContext.BatchUpdate(entities, memberLamda, userName);
        }
    	
        public void BatchUpdate<T2>(TEntity entityData, Expression<Func<TEntity, T2>> memberSetterLamda, TEntity conditionData, Expression<Func<TEntity, T2>> memberConditionLamda, string userName)
        {    
            DataContext.BatchUpdate(entityData, memberSetterLamda, conditionData, memberConditionLamda, userName);
        }
    
        public void BatchDelete(Expression<Func<TEntity, object>> property, object value, bool notEqual = false)
        {
            DataContext.DeleteWhere(property, value, notEqual);
        }
    
        public void DeleteAll()
        {
            DataContext.DeleteAll<TEntity>();
        }
    
        #region IRepository
    
        /// <summary>
        /// Returns list of known entities in EF memory
        /// </summary>
        /// <param name="entityState">Specify state of entity which should be returned</param>
        /// <returns>List of known entities in specified state</returns>
        public IEnumerable<TEntity> Known(EntityState? entityState = null)
        {
           return DataContext.ChangeTracker.Entries<TEntity>().Where(i => entityState == null || i.State == entityState.Value).Select(i => i.Entity);
        }
    
        /// <summary>
        /// Method select and return all items which are not deleted and apply all available query filters on them.
        /// </summary>
        /// <returns>Typed enumerable of items.</returns>
        public IQueryable<TEntity> All()
        {
            return prefilteringManager.ApplyFilters(All(false));
        }
    
        /// <summary>
        /// Method select and return all items which are not deleted.
        /// </summary>
        /// <returns>Typed enumerable of items.</returns>
        public IQueryable<TEntity> AllPure()
        {
            return All(false);
        }
    
        /// <summary>
        /// Method select and return all items.
        /// </summary>
        /// <param name="deleted">Filter data: <c>true</c> - deleted data, <c>false</c> - undeleted data, <c>null</c> - both</param>
        /// <returns>Typed enumerable of filtered items.</returns>
        public virtual IQueryable<TEntity> All(bool? deleted)
        {
    		IQueryable<TEntity> result = DbSet;
    
            //if (deleted.HasValue)
            //{
                //result = result.Where(x => x.Deleted == deleted.Value);
            //}
    
    		return result;
        }
    
    	/// <summary>
        /// Method select one item by id.
        /// </summary>
        /// <param name="id">Id of item which will be select. Id has to be graeter than 0.</param>
    	/// <exception cref="ArgumentOutOfRangeException">When id is lesser than 1.</exception>
        /// <returns>Return selected item or null.</returns>
    	public virtual TEntity Get(int id)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException("id", "Id has to be grater than 0!");
            }
    
            return DbSet.FirstOrDefault(/*x => x.Id == id*/);
        }
    
    	/// <summary>
    	/// Creates new instance and add it to context.
    	/// </summary>
    	/// <returns>Returns brand new instance added to data context.</returns>
    	public virtual TEntity New()
    	{
    		var entity = new TEntity();
    		Add(entity);
    		return entity;
    	}
    
    	/// <summary>
    	/// Creates new specified instance and add it to context.
    	/// </summary>
    	/// <returns>Returns brand new specified instance added to data context.</returns>
    	public virtual T New<T>() where T : TEntity, new()
    	{
    		var entity = new T();
    		Add(entity);
    		return entity;
    	}
    
    	/// <summary>
        /// Method add new item to context.
        /// </summary>
        /// <param name="entity">Item which will be added.</param>
        public virtual TEntity Add(TEntity entity)
        {
            DbSet.Add(entity);
    		return entity;
        }
    
    	/// <summary>
        /// Method remove exist item from context.
        /// </summary>
        /// <param name="entity">Item which will be removed.</param>
        public virtual void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }
    
        /// <summary>
        /// Method remove exist items from context.
        /// </summary>
        /// <param name="entities">Items which will be removed.</param>
        public virtual void Remove(IEnumerable<TEntity> entities)
        {
            entities.ForEach(Remove);
        }
    
    	/// <summary>
        /// Method remove exist item from context.
        /// </summary>
    	/// <exception cref="ArgumentOutOfRangeException">When id is lesser than 1.</exception>
        /// <param name="id">Item Id which will be removed.</param>
        public void Remove(int id)
        {
            TEntity toRemove = Get(id);
    
            if (toRemove != null)
            {
                Remove(toRemove);
            }
        }
    
    	#endregion IRepository
    }
}

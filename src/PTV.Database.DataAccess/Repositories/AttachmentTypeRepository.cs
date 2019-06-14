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

using PTV.Database.Model.Models;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Framework;

namespace PTV.Database.DataAccess.Repositories
{
    /// <summary>
    /// Strongly typed repository interface for extending basic generic repository.
    /// </summary>
    [RegisterService(typeof(IAttachmentTypeRepository), RegisterType.Transient)]
    [RegisterService(typeof(IRepository<AttachmentType>), RegisterType.Transient)]
    internal partial class AttachmentTypeRepository : Repository<AttachmentType>, IAttachmentTypeRepository
    {
    	#region Ctor
    	/// <summary>
        /// Default constructor for base repository.
        /// </summary>
    	/// <exception cref="ArgumentNullException">When context is null.</exception>
        /// <param name="context">Data context for repository instance.</param>
    	/// <param name="prefilteringManager">Prefiltering manager for query filters.</param>
    	public AttachmentTypeRepository(PtvDbContext context, IPrefilteringManager prefilteringManager)
    		: base(context, prefilteringManager)
    	{
    	}
    
    	#endregion Ctor
    }
}

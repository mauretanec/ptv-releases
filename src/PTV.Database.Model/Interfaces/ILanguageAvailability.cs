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
using PTV.Database.Model.Models;

namespace PTV.Database.Model.Interfaces
{
    internal interface ILanguageAvailabilityBase : ILanguageAvailability
    {
        // TODO: this interface should be removed, since the computed ID property
        // causes slowness in Linq queries (they cannot be translated to SQL) and
        // the remaining interface is redundant. ILanguageAvailability should be
        // used instead.
        Guid Id { get; }
    }

    internal interface ILanguageAvailability : IAuditing
    {
        Guid LanguageId { get; set; }

        Guid StatusId { get; set; }

        PublishingStatusType Status { get; set; }

        DateTime? Reviewed { get; set; }

        string ReviewedBy { get; set; }
        
        DateTime? PublishAt { get; set; }

        DateTime? ArchiveAt { get; set; }
        
        string SetForArchivedBy { get; set; }
        DateTime? SetForArchived { get; set; }
        
        /// <summary>
        /// The date of last failed scheduled publishing.
        /// </summary>
        DateTime? LastFailedPublishAt { get; set; }
    }

    internal interface IMultilanguagedEntity
    {
        IEnumerable<ILanguageAvailability> LanguageAvailabilitiesReference { get; }
    }

    internal interface IMultilanguagedEntity<T> : IMultilanguagedEntity where T : ILanguageAvailability
    {
        ICollection<T> LanguageAvailabilities { get; set; }
    }
}

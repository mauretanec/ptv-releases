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
import { Schema, arrayOf, define } from 'normalizr'
import { CommonSchemas } from '../../../../Common/Schemas';
import { addressTypes } from '../../../../Common/Helpers/types';

const organizationSchema = new Schema('organizations', { idAttribute: organization => organization.id });
const filteredOrganizationSchema = new Schema('filteredOrganizations', { idAttribute: organization => organization.id });

organizationSchema.define({
  children: arrayOf(organizationSchema),
  municipality: CommonSchemas.MUNICIPALITY,
  emails: CommonSchemas.EMAIL_ARRAY,
  phoneNumbers: CommonSchemas.PHONE_NUMBER_ARRAY,
  webPages: CommonSchemas.WEB_PAGE_ARRAY, 
  business: CommonSchemas.BUSINESS,
  [addressTypes.POSTAL]: CommonSchemas.ADDRESS_ARRAY,
  [addressTypes.VISITING]: CommonSchemas.ADDRESS_ARRAY,
});

filteredOrganizationSchema.define({
  children: arrayOf(filteredOrganizationSchema)
});

export const OrganizationSchemas = {
  ORGANIZATION: organizationSchema,
  ORGANIZATION_ARRAY: arrayOf(organizationSchema),
  FILTERED_ORGANIZATION_ARRAY: arrayOf(filteredOrganizationSchema)
}
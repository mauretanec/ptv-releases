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
import { schema } from 'normalizr'
import { CommonSchemas } from '../../../../Common/Schemas'
import { addressTypes } from '../../../../Common/Helpers/types'
import { IntlSchemas } from '../../../../../Intl/Schemas'

const defineSchema = (localizable) => {
  const organizationSchema = new schema.Entity('organizations', {
    children: new schema.Array(organizationSchema),
    municipality: CommonSchemas.MUNICIPALITY,
    emails: CommonSchemas.EMAIL_ARRAY,
    phoneNumbers: CommonSchemas.PHONE_NUMBER_ARRAY,
    webPages: CommonSchemas.WEB_PAGE_ARRAY,
    business: CommonSchemas.BUSINESS,
    [addressTypes.POSTAL]: CommonSchemas.ADDRESS_ARRAY,
    [addressTypes.VISITING]: CommonSchemas.ADDRESS_ARRAY,
    translation: IntlSchemas.TRANSLATED_ITEM,
    security: CommonSchemas.SECURITY
  }, {
    meta: { localizable: localizable }
  })
  const organizationSchema_v2 = new schema.Entity('organizations')
  const filteredOrganizationSchema = new schema.Entity('filteredOrganizations')

  filteredOrganizationSchema.define({
    children: new schema.Array(filteredOrganizationSchema)
  })

  return {
    organizationSchema,
    organizationSchema_v2,
    filteredOrganizationSchema
  }
}

const schemas = defineSchema(true)
const schemasCommon = defineSchema(false)

export const OrganizationSchemas = {
  ORGANIZATION: schemas.organizationSchema,
  ORGANIZATION_GLOBAL: schemasCommon.organizationSchema,
  ORGANIZATION_ARRAY: new schema.Array(schemas.organizationSchema),
  ORGANIZATION_ARRAY_GLOBAL: new schema.Array(schemasCommon.organizationSchema),
  FILTERED_ORGANIZATION_ARRAY: new schema.Array(schemas.filteredOrganizationSchema),
  ORGANIZATION_V2: schemas.organizationSchema_v2,
  ORGANIZATION_ARRAY_V2: new schema.Array(schemas.organizationSchema_v2)
}

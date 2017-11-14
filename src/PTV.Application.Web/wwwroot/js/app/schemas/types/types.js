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
import { defineChildrenSchema } from 'schemas/finto'

const publishingStatus = new schema.Entity('publishingStatuses')
const chargeType = new schema.Entity('chargeTypes')
const channelType = new schema.Entity('channelTypes')
const phoneNumberType = new schema.Entity('phoneNumberTypes')
const webPageType = new schema.Entity('webPageTypes')
const printableFormUrlType = new schema.Entity('printableFormUrlTypes')
const serviceHourType = new schema.Entity('serviceHourType')
const areaInformationType = new schema.Entity('areaInformationTypes')
const areaType = new schema.Entity('areaTypes')
const keyword = new schema.Entity('keywords')
const serviceChannelConnectionType = new schema.Entity('serviceChannelConnectionTypes')
const serviceType = new schema.Entity('serviceTypes')
const fundingType = new schema.Entity('fundingTypes')
const provisionType = new schema.Entity('provisionTypes')
const organizationType = new schema.Entity('organizationTypes')

defineChildrenSchema(organizationType)

export const TypeSchemas = {
  PUBLISHING_STATUS: publishingStatus,
  PUBLISHING_STATUS_ARRAY: new schema.Array(publishingStatus),
  CHARGE_TYPE:chargeType,
  CHARGE_TYPE_ARRAY: new schema.Array(chargeType),
  AREA_INFORMATION_TYPE: areaInformationType,
  AREA_INFORMATION_TYPE_ARRAY: new schema.Array(areaInformationType),
  AREA_TYPE: areaType,
  AREA_TYPE_ARRAY: new schema.Array(areaType),
  KEYWORD_TYPE: keyword,
  KEYWORD_ARRAY: new schema.Array(keyword),
  SERVICE_CHANNEL_CONNECTION_TYPE: serviceChannelConnectionType,
  SERVICE_CHANNEL_CONNECTION_TYPE_ARRAY: new schema.Array(serviceChannelConnectionType),
  PHONE_NUMBER_TYPE: phoneNumberType,
  PHONE_NUMBER_TYPE_ARRAY: new schema.Array(phoneNumberType),
  CHANNEL_TYPE: channelType,
  CHANNEL_TYPE_ARRAY: new schema.Array(channelType),
  WEB_PAGE_TYPE: webPageType,
  WEB_PAGE_TYPE_ARRAY: new schema.Array(webPageType),
  PRINTABLE_FORM_TYPE: printableFormUrlType,
  PRINTABLE_FORM_TYPE_ARRAY: new schema.Array(printableFormUrlType),
  SERVICE_HOUR_TYPE: serviceHourType,
  SERVICE_HOUR_TYPE_ARRAY: new schema.Array(serviceHourType),
  SERVICE_TYPE: serviceType,
  SERVICE_TYPE_ARRAY: new schema.Array(serviceType),
  PROVISION_TYPE: provisionType,
  PROVISION_TYPE_ARRAY: new schema.Array(provisionType),
  FUNDING_TYPE: fundingType,
  FUNDING_TYPE_ARRAY: new schema.Array(fundingType),
  ORGANIZATION_TYPE: organizationType,
  ORGANIZATION_TYPE_ARRAY: new schema.Array(organizationType)
}

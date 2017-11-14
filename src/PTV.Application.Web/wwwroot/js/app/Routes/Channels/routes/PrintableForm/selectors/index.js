import { createSelector } from 'reselect'
import { fromJS } from 'immutable'
import {
  getId,
  getPhones,
  getAttachments,
  getDeliveryAddress,
  getEmails,
  getDescription,
  getFormFiles,
  getConnectionType,
  getChannelProperty
} from 'Routes/Channels/selectors'
import { getAreaInformationOrDefaultEmpty, getEntityLanguageAvailabilities, getOrganization } from 'Routes/selectors'

export const getPrintableForm = createSelector([
  getId,
  getChannelProperty('name'),
  getOrganization,
  getChannelProperty('shortDescription'),
  getDescription,
  getPhones,
  getAttachments,
  getEmails,
  getFormFiles,
  getAreaInformationOrDefaultEmpty,
  getDeliveryAddress,
  getEntityLanguageAvailabilities,
  getConnectionType,
  getChannelProperty('formReceiver'),
  getChannelProperty('formIdentifier')
], (
  id,
  name,
  organization,
  shortDescription,
  description,
  phoneNumbers,
  attachments,
  emails,
  formFiles,
  areaInformation,
  deliveryAddress,
  languagesAvailabilities,
  connectionType,
  formReceiver,
  formIdentifier
) => fromJS({
  id,
  name,
  organization,
  shortDescription,
  description,
  phoneNumbers,
  attachments,
  emails,
  formFiles,
  areaInformation,
  deliveryAddress,
  languagesAvailabilities,
  connectionType,
  formReceiver,
  formIdentifier
}))

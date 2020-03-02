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
import { createSelector } from 'reselect'
import { getFormValueWithPath } from 'selectors/base'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'
import { EntitySelectors } from 'selectors'
import { Map } from 'immutable'

const getFormOrganizationIds = createSelector(
  getFormValueWithPath(props => 'organizationIds'),
  organizationIds => organizationIds || Map()
)

const getFormOrganizations = createSelector(
  [getFormOrganizationIds, EntitySelectors.organizations.getEntities],
  (ids, organizations) => {
    return organizations.filter((value, key) => ids.some(id => id === key))
  }
)

const getTranslatedOrganizationNames = createTranslatedListSelector(
  getFormOrganizations, {
    nameAttribute: 'displayName',
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getAreAllOrganizationsSelected = createSelector(
  getFormOrganizationIds,
  organizationIds => !organizationIds || organizationIds.size === 0
)

export const getSelectedOrganizations = createSelector(
  getTranslatedOrganizationNames,
  organizationNames => {
    const result = []
    organizationNames.forEach((entry, key) => {
      result.push({
        label: entry.get('displayName'),
        value: key
      })
    })
    return result
  }
)

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
import { Map, fromJS } from 'immutable'
import { camelizeKeys } from 'humps'
import EntitySelectors from 'selectors/entities/entities'

export const getUser = () => camelizeKeys(g_userInfo)
export const getApplicationVersion = () => getAppVersion()
export const getMapDNSes = () => fromJS(camelizeKeys(g_mapDNSnames)) || Map()

export const getUserInfo = createSelector(
    getUser,
    user => Map().mergeDeep(user)
)
export const getUserRoleName = createSelector(
    getUserInfo,
    user => user.get('role')
)
export const getUserName = createSelector(
    getUserInfo,
    user => user.get('name')
)
export const getUserOrganization = createSelector(
    getUserInfo,
    user => user.get('userOrganization') || null
)

export const getUserOrganizationRoles = createSelector(
  EntitySelectors.organizationRoles.getEntities,
  organizationRoles => organizationRoles
    .map(or => or.get('role').toLowerCase())
)

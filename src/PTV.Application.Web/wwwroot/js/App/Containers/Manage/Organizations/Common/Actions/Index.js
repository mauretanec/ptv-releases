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

import { CALL_API, Schemas } from '../../../../../Middleware/Api';
import { onEntityInputChange, onEntityListChange, onEntityObjectChange, onEntityAdd, fakeApiCall, onLocalizedEntityAdd, onLocalizedEntityListChange } from '../../../../Common/Actions';
import * as CommonSelectors from '../Selectors';
import { OrganizationSchemas } from '../../Organization/Schemas';
import * as CommonActions from '../../../../../Containers/Common/Actions';

export function onOrganizationEntityAdd(property, entity, id, replace= false, schema= OrganizationSchemas.ORGANIZATION) {
	return () => onEntityAdd({id: id, [property]: entity}, schema, replace);
}

export function onOrganizationEntityReplace(property, entity, id, schema= OrganizationSchemas.ORGANIZATION) {
	return () => fakeApiCall({id: id, [property]: entity}, schema);
}

export function onOrganizationInputChange(input, id, value, isSet, entity='organizations') {
	return () => onEntityInputChange(entity, id, input, value, isSet)
}

export function onOrganizationObjectChange(id, object, isSet, entity='organizations') {
	return () => onEntityObjectChange([entity], id, object, isSet)
}

export function onOrganizationListChange(input, id, value, isAdd, entity='organizations') {
	return () => onEntityListChange(entity, id, input, value, isAdd)
}

export function onLocalizedOrganizationEntityAdd(propetry, entity, id, language, schema= OrganizationSchemas.ORGANIZATION) {
	return () => onLocalizedEntityAdd({id: id, [propetry]: entity}, schema, language);
}


export const ORGANIZATION_SET_ORGANIZATION_ID = 'ORGANIZATION_SET_ORGANIZATION_ID';

export function setOrganizationId(organizationId) {
	return () => ({
		type: ORGANIZATION_SET_ORGANIZATION_ID,
		pageSetup:{
			id: organizationId,
			keyToState: "organization",
		}
	});
}

export const organizationCall = (endpoint, data) => {
	return CommonActions.apiCall(['organization', 'all'],
		 { endpoint, data },
		 [OrganizationSchemas.ORGANIZATION_ARRAY],
		 OrganizationSchemas.ORGANIZATION, 'organization');
}
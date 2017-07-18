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
import React, {PropTypes, Component} from 'react';
import {connect} from 'react-redux';
import { injectIntl } from 'react-intl';

// components
import { DigitalAuthorizationTree } from '../../../../Common/FintoTree';

// actions
import * as commonServiceAndChannelActions from '../../Common/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonServiceAndChannelsSelectors from '../Selectors';

// Validators
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators';

const getIsSelectedDigitalAuthorization = (relationId) => (state, props) => {
    const daProps = {...props, id: relationId, digitalAuthorizationId: props.nodeId};
    return CommonServiceAndChannelsSelectors.getIsSelectedDigitalAuthorization(state, daProps)
}; 

const getSelectedDigitalAuthorizations = (relationId) => (state, props) => {
    return CommonServiceAndChannelsSelectors.getSelectedDigitalAuthorizationsWithoutTop(state, {...props, id: relationId})
}; 

const ServiceAndChannelDigitalAuthorization = ({ messages, readOnly, language, actions, relationChannelId, keyToState, intl }) => {
   
    const onListChange = (input) => (value, isAdd) => {
        actions.onChannelRelationDigitalAuthorizationWithChildrenChange(input, relationChannelId, value, isAdd);      
    }
    
    const onListRemove = (input) => (value) => {
        actions.onChannelRelationDigitalAuthorizationWithChildrenChange(input, relationChannelId, value, false); 
    }
      
    return (
            <div className="form-group">
                <DigitalAuthorizationTree
                    treeViewClass = 'col-xs-6'
                    resultsClass = 'col-xs-6'                    
                    labelTooltip = { intl.formatMessage(messages.tooltip) }
                    label = { intl.formatMessage(messages.title) }
                    onNodeSelect = { onListChange() }
                    onNodeRemove = { onListRemove() }
                    treeTargetHeader={ intl.formatMessage(messages.targetListHeader) }
                    readOnly = { readOnly }
                    isSelectedSelector = { getIsSelectedDigitalAuthorization(relationChannelId) }
                    getSelectedSelector = { getSelectedDigitalAuthorizations(relationChannelId) }
                    language = { language }
                    nodeShowTooltip = { true }
                    entityId = { relationChannelId }
                />
            </div>
    );
}

const actions = [
    commonServiceAndChannelActions
];

export default connect(null, mapDispatchToProps(actions))(injectIntl(ServiceAndChannelDigitalAuthorization));




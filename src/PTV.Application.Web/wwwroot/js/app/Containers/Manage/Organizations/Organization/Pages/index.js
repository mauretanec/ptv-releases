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
import React, { PropTypes, Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

/// App Containers
import StepContainer from '../../../../Common/StepContainer';
import PageContainer from '../../../../Common/PageContainer';

import Step1 from './Steps/Step1';
import OrganizationDeleteDialog from '../../Common/Pages/OrganizationDeleteDialog'

//Actions
import * as organizationActions from '../Actions';
import * as commonActions from '../../../../Common/Actions';

/// Styles
import '../../../../Common/Styles/StepContainer.scss';

//Messages
import * as Messages from '../Messages';

//Selectors
import * as CommonOrganizationSelectors from '../../Common/Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';
import { ElectronicChannelForm } from 'appComponents'

const ManageContainer = props => {

        const renderDeleteConfirmation = () =>{
            return <OrganizationDeleteDialog keyToState={ props.keyToState }/>
        }
        const { step1IsFetching, step1AreDataValid, keyToState } = props;

        return (
          <div>
            <ElectronicChannelForm />
            <PageContainer {...props} className="card service-page"
                    confirmDialogs = { [{ type: 'delete', messages: [Messages.deleteMessages.buttonOk, Messages.deleteMessages.buttonCancel, Messages.deleteMessages.text] },
                                        { type: 'cancel', messages: [Messages.cancelMessages.buttonOk, Messages.cancelMessages.buttonCancel, Messages.cancelMessages.text] },
                                        { type: 'save', messages: [Messages.saveDraftMessages.buttonOk, Messages.saveDraftMessages.buttonCancel, Messages.saveDraftMessages.text] },
                                        { type: 'withdraw', messages: [Messages.withdrawMessages.buttonOk, Messages.withdrawMessages.buttonCancel, Messages.withdrawMessages.text] },
                                        { type: 'restore', messages: [Messages.restoreMessages.buttonOk, Messages.restoreMessages.buttonCancel, Messages.restoreMessages.text] },
                                        { type: 'goBack', messages: [Messages.goBackMessages.buttonOk, Messages.goBackMessages.buttonCancel, Messages.goBackMessages.text] }
                                      ] }
                    renderCustomDialog= { renderDeleteConfirmation }
                    readOnly={ props.readOnly }
                    keyToState = { keyToState }
                    isTranslatable= { true }
                    deleteAction={ props.actions.deleteOrganization }
                    withdrawAction={ props.actions.withdrawOrganization }
                    saveAction={ props.actions.saveAllChanges }
                    restoreAction={ props.actions.restoreOrganization }
                    lockAction={ props.actions.lockOrganization }
                    unLockAction={ props.actions.unLockOrganization }
                    isLockedAction={ props.actions.isOrganizationLocked }
                    isEditableAction={ props.actions.isOrganizationEditable }
                    removeServerResultAction = { props.actions.removeServerResult }
                    basePath = '/frontpage'
                    entitiesType='organizations'
                    getEntityStatusSelector = { CommonOrganizationSelectors.getPublishingStatus }
                    getUnificRootIdSelector = { CommonOrganizationSelectors.getUnificRootId }
                    statusEndpoint = 'organization/GetOrganizationStatus'
                    invalidateAllSteps= { props.actions.cancelAllChanges }
                    steps= { [{
                        mainTitle: Messages.messages.mainTitle,
                        mainTitleView: Messages.messages.mainTitleView,
                        mainText: Messages.messages.mainDescription,
                        mainTextView: Messages.messages.mainDescriptionView,
                        subTitle: Messages.messages.subTitleStep1,
                        subTitleView: Messages.messages.subTitleViewStep1,
                        saveStepAction: props.actions.saveStep1Changes,
                        loadAction: props.actions.getStep1,
                        isFetching: step1IsFetching,
                        areDataValid: step1AreDataValid,
                        child: Step1
                        }
                     ] }>
        </PageContainer>
        </div>
       );
    }

ManageContainer.propTypes = {
            actions: PropTypes.object
        };

function mapStateToProps(state, ownProps) {
    const keyToState = 'organization';
    return {
       step1IsFetching: CommonSelectors.getStep1isFetching(state, {keyToState}),
       step1AreDataValid: CommonSelectors.getStep1AreDataValid(state, { keyToState }),
       keyToState
  }
}

const actions = [
    organizationActions,
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(ManageContainer);

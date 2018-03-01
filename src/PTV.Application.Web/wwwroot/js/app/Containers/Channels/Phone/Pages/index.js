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
import { Map, List } from 'immutable';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';

// actions
import * as channelActions from '../Actions';
import * as commonActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// messages
import * as CommonMessages from '../../Common/Messages';
import * as Messages from '../Messages';
import { channelServicesMessages } from '../../Common/Messages';

// styles
import '../../Common/Styles/Container.scss';

// components
import PageContainer from '../../../Common/PageContainer';
import Step1 from './Steps1';
import Step2 from './Steps2';
import ChannelServiceStep from '../../Common/Pages/channelServiceStep';

// Selectors
import * as CommonChannelSelectors from '../../Common/Selectors';
import * as CommonSelectors from '../../../Common/Selectors';

// types
import { channelTypes } from '../../../Common/Enums';

const Container = props => {
        const { step1IsFetching, step2IsFetching, step1AreDataValid, step2AreDataValid, channelServiceStepIsFething, channelServiceStepAreDataValid, readOnly } = props;

        return (
            <PageContainer { ...props } className="card channel-page"
                    confirmDialogs = { [ { type: 'cancel', messages: [Messages.cancelMessages.buttonOk, Messages.cancelMessages.buttonCancel, Messages.cancelMessages.text] },
                                         { type: 'delete', messages: [Messages.deleteMessages.buttonOk, Messages.deleteMessages.buttonCancel, Messages.deleteMessages.text] },
                                         { type: 'save', messages: [CommonMessages.saveDraftMessages.buttonOk, CommonMessages.saveDraftMessages.buttonCancel, CommonMessages.saveDraftMessages.text] },
                                         { type: 'withdraw', messages: [Messages.withdrawMessages.buttonOk, Messages.withdrawMessages.buttonCancel, Messages.withdrawMessages.text] },
                                         { type: 'restore', messages: [Messages.restoreMessages.buttonOk, Messages.restoreMessages.buttonCancel, Messages.restoreMessages.text] },
                                         { type: 'goBack', messages: [CommonMessages.goBackMessages.buttonOk, CommonMessages.goBackMessages.buttonCancel, CommonMessages.goBackMessages.text] }
                                         ] }
                    readOnly={ readOnly }
                    isTranslatable= { true }
                    deleteAction={ props.actions.deleteChannel }
                    withdrawAction={ props.actions.withdrawChannel }
                    restoreAction={ props.actions.restoreChannel }
                    saveAction={ props.actions.saveAllChanges }
                    lockAction={ props.actions.lockChannel }
                    unLockAction={ props.actions.unLockChannel }
                    isLockedAction={ props.actions.isChannelLocked }
                    isEditableAction={ props.actions.isChannelEditable }
                    removeServerResultAction = { props.actions.removeServerResult }
                    invalidateAllSteps= { props.actions.cancelAllChanges }
                    getEntityStatusSelector = { CommonChannelSelectors.getPublishingStatus }
                    entitiesType='channels'
                    getUnificRootIdSelector = { CommonChannelSelectors.getUnificRootId }
                    statusEndpoint = 'channel/GetChannelStatus'
                    basePath='/frontpage'
                    steps= { [{
                        mainTitle: Messages.messages.mainTitle,
                        mainTitleView: Messages.messages.mainTitleView,
                        mainText: Messages.messages.mainText,
                        mainTextView: Messages.messages.mainTextView,
                        subTitle: Messages.messages.subTitle1,
                        subTitleView: Messages.messages.subTitle1View,
                        saveStepAction: props.actions.saveStep1Changes,
                        loadAction: props.actions.getStep1,
                        isFetching: step1IsFetching,
                        areDataValid: step1AreDataValid,
                        child: Step1
                        }
                        ,{
                        subTitle: Messages.messages.subTitle2,
                        subTitleView: Messages.messages.subTitle2View,
                        saveStepAction: props.actions.saveStep2Changes,
                        loadAction: props.actions.getStep2,
                        isFetching: step2IsFetching,
                        areDataValid: step2AreDataValid,
                        child: Step2
                        },{
                        subTitle: channelServicesMessages.title,
                        subTitleView: channelServicesMessages.title,
                        loadAction: props.actions.getChannelServiceStep,
                        isFetching: channelServiceStepIsFething,
                        areDataValid: channelServiceStepAreDataValid,
                        readOnlyVisible: true,
                        child: ChannelServiceStep,
                        stepKey: 'channelServiceStep'
                        }
                        ]}/>
       );
}

function mapStateToProps(state, ownProps) {
    const keyToState = channelTypes.PHONE;
   return {
       step1IsFetching: CommonSelectors.getStep1isFetching(state,{keyToState}),
       step2IsFetching: CommonSelectors.getStep2isFetching(state,{keyToState}),
       step1AreDataValid: CommonSelectors.getStep1AreDataValid(state, { keyToState, simpleView:ownProps.simpleView }),
       step2AreDataValid: CommonSelectors.getStep2AreDataValid(state, { keyToState, simpleView:ownProps.simpleView }),
       keyToState,
       channelServiceStepIsFething: CommonSelectors.getChannelServiceStepIsFetching(state, { keyToState }),
       channelServiceStepAreDataValid: CommonSelectors.getChannelServiceStepAreDataValid(state, { keyToState })
  }
}

const actions = [
     channelActions,
     commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(Container);

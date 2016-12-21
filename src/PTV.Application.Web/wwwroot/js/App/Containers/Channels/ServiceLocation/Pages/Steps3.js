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

// messages
import * as Messages from '../Messages';

// components
import ChannelAddresses from '../../Common/Pages/channelAddresses';
import LanguageLabel from '../../../Common/languageLabel';

// types
import { channelTypes } from '../../Common/Helpers/types';
import { addressTypes } from '../../../Common/Helpers/types';

// actions
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';
import * as locationChannelActions from '../Actions';

// selectors
import * as CommonSelectors from '../../../Common/Selectors';

export const Step3 = ({readOnly, keyToState, language, translationMode, splitContainer}) => {
    const sharedProps = { readOnly, translationMode, language, keyToState, splitContainer };
    return (
        <div className="step-3">
            <LanguageLabel {...sharedProps}
            />

            <ChannelAddresses {...sharedProps}
                addressType= { addressTypes.VISITING }
            />
            <ChannelAddresses {...sharedProps}
                addressType= { addressTypes.POSTAL }
            />
        </div>
    );
}

function mapStateToProps(state, ownProps) {
    const keyToState = channelTypes.SERVICE_LOCATION;
  return { 
      keyToState      
  }
}

const actions = [
    locationChannelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(Step3);

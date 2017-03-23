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
import React, {PropTypes} from 'react';
import {connect} from 'react-redux';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import Laws from '../../../Common/Laws';

// selectors
import * as ServiceSelectors from '../../Service/Selectors';

export const ServiceGeneralDecriptionLaws = ({messages, readOnly, language, translationMode, laws, actions, shouldValidate, generalDescriptionName}) => {

    const onAddLaw = (entity) => {
        actions.onLawAdd(entity, language);
    }

    const onRemoveLaw = (id) => {
        actions.onListChange('laws', id, language);
    }

    const sharedProps = { readOnly:true , language, translationMode:'none' };
    return(
        laws.size > 0 ?
        <div className = "readonly">
            <Laws {...sharedProps}
                messages= { messages }
                items= { laws }
                shouldValidate= { shouldValidate }
                onAddLaw= { onAddLaw }
                onRemoveLaw = { onRemoveLaw }
                collapsible= { false }
                withName
                generalDescriptionName = { generalDescriptionName }
            />
        </div>
        : null
    )
}

ServiceGeneralDecriptionLaws.propTypes = {
    shouldValidate: PropTypes.bool,
};

ServiceGeneralDecriptionLaws.defaultProps = {
    shouldValidate: true,
};

function mapStateToProps(state, ownProps) {

  return {
      laws: ServiceSelectors.getLawsFromGeneralDescription(state, ownProps),
      generalDescriptionName: ServiceSelectors.getServiceNameFromGeneralDescriptionLocale(state, ownProps),
  }
}

const actions = [
    serviceActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(ServiceGeneralDecriptionLaws);

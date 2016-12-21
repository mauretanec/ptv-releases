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
import { injectIntl } from 'react-intl';
import ImmutablePropTypes from 'react-immutable-proptypes';
import shortId from 'shortid';
import { List } from 'immutable';

// components
import StreetAddress from './StreetAddressContainer';
import { PTVAddItem } from '../../../Components';

// selectors
import * as CommonSelectors from '../Selectors';

// types
import { addressTypes } from '../../Common/Helpers/types';

// messages
import { deliveryAddressMessagges, postalAddressMessagges, visitingAddressMessagges } from '../../Channels/Common/Messages';


const messages = {
    [addressTypes.DELIVERY]: deliveryAddressMessagges, 
    [addressTypes.POSTAL]: postalAddressMessagges, 
    [addressTypes.VISITING]: visitingAddressMessagges, 
}

export const Addresses = ({ readOnly, coordinatesHidden, language, translationMode, splitContainer, showTypeSelection, receiver, addresses,
                            shouldValidate, isAdditionalInformationVisible, isAdditionalInformationTextAreaVisible, type, onAddAddress, onRemoveAddress, intl, collapsible, addressType}) => {
    const sharedProps = { readOnly, language, translationMode, splitContainer };
    const renderAddress = (id, index, isNew) => {
        return (
            <StreetAddress {...sharedProps}
                showTypeSelection= { showTypeSelection }
                type= { type }
                addressId= { id }
                isNew= { isNew }
                onAddAddress= { onAddAddress }
                shouldValidate={ shouldValidate }
                isAdditionalInformationVisible= { isAdditionalInformationVisible }
                isAdditionalInformationTextAreaVisible= { isAdditionalInformationTextAreaVisible }
                customMessages= { messages[addressType] }
                coordinatesHidden= { coordinatesHidden }
            />
	    );
    };

    const onAddButtonClick = () => {
        onAddAddress(addresses.size === 0 ? [{ id: shortId.generate(), streetType: 'Street' }, { id: shortId.generate(), streetType: 'Street' }] : [{ id: shortId.generate(), streetType: 'Street' }]);
    }

    const props = List.isList(addresses) ? { items: addresses } : { itemId: addresses };
    return(
            <PTVAddItem
                customComponentsToRender= { receiver }
                readOnly = { readOnly && translationMode == 'none' }
                renderItemContent = { renderAddress }
                messages = {{ "label": intl.formatMessage(messages[addressType].title) }}
                onAddButtonClick = { onAddButtonClick }
                onRemoveItemClick = { onRemoveAddress }
                collapsible = { collapsible && translationMode == 'none' }
                multiple = { translationMode == 'none' }
                {...props} />
    )
}

Addresses.propTypes = {
        addresses: ImmutablePropTypes.iterable.isRequired,
        showTypeSelection: PropTypes.bool,
        shouldValidate: PropTypes.bool,
        isAdditionalInformationVisible: PropTypes.bool,
        isAdditionalInformationTextAreaVisible: PropTypes.bool,
        readOnly: PropTypes.bool,
        type: PropTypes.string,
        onAddAddress: PropTypes.func.isRequired,
        onRemoveWebPage: PropTypes.func,
        coordinatesHidden: PropTypes.bool,
    };

Addresses.defaultProps = {
  showTypeSelection: false,
  shouldValidate: false,
  isAdditionalInformationVisible: false,
  isAdditionalInformationTextAreaVisible: false,
  coordinatesHidden: false,
}

export default injectIntl(Addresses);

                    

//                     <div className="row form-group">
//                         <PTVTextArea
//                             componentClass="col-xs-12"
//                             minRows={ 2 }
//                             maxLength={ 150 }
//                             label = { formatMessage(Messages.deliveryAddressMessagges.descriptionTitle) }
//                             tooltip = { formatMessage(Messages.deliveryAddressMessagges.descriptionInfo) }
//                             placeholder = { formatMessage(Messages.deliveryAddressMessagges.descriptionPlaceholder) }
//                             value={ step1Form.deliveryAddressDescription }
//                             blurCallback={ this.onDeliveryAddressDescriptionChange }
//                             name="DeliveryAddressDescription"
//                             readOnly= { readOnly }
//                         />
//                     </div>

//                 </div>
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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Accessibility } from 'util/redux-form/fields'
import styles from './styles.scss'
import {
  getAddressType,
  getHasAccessibilityRegisterId
} from 'selectors/addresses'
import { addressTypesEnum } from 'enums'
import { defineMessages } from 'util/react-intl'
import asContainer from 'util/redux-form/HOC/asContainer'

const messages = defineMessages({
  title: {
    id: 'Accessibility.Readonly.Title',
    defaultMessage: 'Käyntiosoitteen esteettömystiedot',
    description: 'Accessibility.Title'
  }
})

const AccessibilityComponent = compose(
  asContainer({
    doNotCompareContainerHead: true,
    overrideReadonly: true,
    uiKey: 'accessibilityInformationContainer'
  })
)(props => (
  <div className={styles.accessibility}>
    <Accessibility {...props} />
  </div>
))

const withAccessibilityInformation = WrappedComponent => {
  const InnerComponent = ({
    addressType,
    hasAccessibilityRegisterId,
    ...rest
  }) => {
    const showLinks = addressType === addressTypesEnum.STREET && rest.index === 0
    return (
      <div>
        <WrappedComponent {...rest} />
        {(hasAccessibilityRegisterId || showLinks) && (
          <AccessibilityComponent showLinks={showLinks} {...rest} title={messages.title} tooltip={null} />
        )}
      </div>
    )
  }
  InnerComponent.propTypes = {
    addressType: PropTypes.string,
    hasAccessibilityRegisterId: PropTypes.bool
  }
  return compose(
    connect((state, ownProps) => ({
      addressType: getAddressType(state, ownProps),
      hasAccessibilityRegisterId: getHasAccessibilityRegisterId(state, ownProps)
    }))
  )(InnerComponent)
}

export default withAccessibilityInformation

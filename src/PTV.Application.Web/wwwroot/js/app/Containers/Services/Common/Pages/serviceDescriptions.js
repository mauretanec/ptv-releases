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
import React, { PropTypes, Component } from 'react'
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// components
import { PTVTextAreaNotEmpty } from '../../../../Components'
import ServiceDescription from './serviceDescription'
import ServiceDescriptionTextEditor from './serviceDescriptionTextEditor'

// actions
import * as serviceActions from '../../Service/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// selectors
import * as CommonServiceSelectors from '../Selectors'
import * as ServiceSelectors from '../../Service/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../Components/PTVValidators'

const ServiceDescriptions = ({
    messages, readOnly, language, translationMode,
    shortDescriptions,
    description,
    serviceUsage,
    userInstruction,
    descriptionFromGeneralDescription,
    backgroundDescriptionFromGeneralDescription,
    serviceUsageFromGeneralDescription,
    userInstructionFromGeneralDescription,
    serviceTypeCode,
    actions, intl }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onServiceInputChange(input, value, language, isSet)
  }

  const serviceUsagePlaceholder = () => {
    switch (serviceTypeCode) {
      case 'Service': return intl.formatMessage(messages.conditionOfServiceUsageServicePlaceholder)
      case 'PermissionAndObligation': return intl.formatMessage(messages.conditionOfServiceUsagePermissionPlaceholder)
      default: return intl.formatMessage(messages.conditionOfServiceUsageServicePlaceholder)
    }
  }

  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const translatableAreaRO = readOnly && translationMode == 'none'
  const generalDescriptionRO = readOnly || translationMode == 'edit'
  const shortDescriptionClass = readOnly || translationMode !== 'none' ? 'col-xs-12' : 'col-lg-6'

  return (
    <div>
      <div className='row form-group'>
        <PTVTextAreaNotEmpty
          componentClass={shortDescriptionClass}
          minRows={3}
          maxLength={150}
          label={intl.formatMessage(messages.shortDescriptionTitle)}
          placeholder={intl.formatMessage(messages.shortDescriptionPlaceholder)}
          tooltip={intl.formatMessage(messages.shortDescriptionTooltip)}
          value={shortDescriptions}
          name='serviceShortDescription'
          blurCallback={onInputChange('shortDescriptions')}
          order={20}
          disabled={translationMode == 'view'}
          readOnly={translatableAreaRO}
          validatedField={messages.shortDescriptionTitle}
          validators={validators}
           />
      </div>

      <ServiceDescriptionTextEditor
        title={intl.formatMessage(messages.descriptionTitle)}
        tooltip={intl.formatMessage(messages.descriptionTooltip)}
        placeHolder={intl.formatMessage(messages.descriptionPlaceholder)}
        readOnly={readOnly}
        translationMode={translationMode}
        name='serviceDescription'
        value={description}
        valueFromGeneralDescription={descriptionFromGeneralDescription}
        blurCallback={onInputChange('description')}
        maxLength={2500}
        language={language}
        validators={descriptionFromGeneralDescription.length > 0 ? [] : validators}
        validatedField={messages.descriptionTitle}
      />

      <ServiceDescriptionTextEditor
        title={intl.formatMessage(messages.conditionOfServiceUsageTitle)}
        tooltip={intl.formatMessage(messages.conditionOfServiceUsageTooltip)}
        placeHolder={serviceUsagePlaceholder()}
        readOnly={readOnly}
        translationMode={translationMode}
        name='conditionOfUsage'
        value={serviceUsage}
        valueFromGeneralDescription={serviceUsageFromGeneralDescription}
        blurCallback={onInputChange('serviceUsage')}
        maxLength={2500}
        validatedField={messages.conditionOfServiceUsageTitle}
        language={language}
              />

      <ServiceDescriptionTextEditor
        title={intl.formatMessage(messages.serviceUserInstructionTitle)}
        tooltip={intl.formatMessage(messages.serviceUserInstructionTooltip)}
        placeHolder={intl.formatMessage(messages.serviceUserInstructionPlaceholder)}
        readOnly={readOnly}
        translationMode={translationMode}
        name='serviceUserInstruction'
        value={userInstruction}
        valueFromGeneralDescription={userInstructionFromGeneralDescription}
        blurCallback={onInputChange('userInstruction')}
        maxLength={2500}
        validatedField={messages.serviceUserInstructionTitle}
        language={language}
              />
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  return {
    shortDescriptions: ServiceSelectors.getShortDescriptions(state, ownProps),
    description: ServiceSelectors.getDescription(state, ownProps),
    serviceUsage: ServiceSelectors.getServiceUsage(state, ownProps),
    userInstruction: ServiceSelectors.getUserInstruction(state, ownProps),
    descriptionFromGeneralDescription: ServiceSelectors.getDescriptionFromGeneralDescriptionLocale(state, ownProps),
    backgroundDescriptionFromGeneralDescription: ServiceSelectors.getBackgroundDescriptionFromGeneralDescriptionLocale(state, ownProps),
    serviceUsageFromGeneralDescription: ServiceSelectors.getServiceUsageFromGeneralDescriptionLocale(state, ownProps),
    userInstructionFromGeneralDescription: ServiceSelectors.getUserInstructionFromGeneralDescriptionLocale(state, ownProps),
    serviceTypeCode:ServiceSelectors.getSelctedServiceTypeCode(state, ownProps)
  }
}

const actions = [
  serviceActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceDescriptions))


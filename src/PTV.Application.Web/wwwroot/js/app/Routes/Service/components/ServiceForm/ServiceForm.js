/* eslint-disable standard/object-curly-even-spacing */
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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch } from 'recompose'
import { reduxForm, formValueSelector, change } from 'redux-form/immutable'
import { mergeInUIState } from 'reducers/ui'
import ServiceBasic from '../ServiceBasic'
import ServiceClassificationAndKeywords from '../ServiceClassificationAndKeywords'
import ServiceProducerInfo from '../ServiceProducerInfo'
import { serviceBasicTransformer } from '../serviceTransformers'
import { defineMessages, injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import { ReduxAccordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import withEntityButtons from 'util/redux-form/HOC/withEntityButtons'
import withEntityHeader from 'util/redux-form/HOC/withEntityHeader'
import withEntityNotification from 'util/redux-form/HOC/withEntityNotification'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withConnectionStep from 'util/redux-form/HOC/withConnectionStep'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import withQualityAgentProvider from 'util/redux-form/HOC/withQualityAgentProvider'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import validate from 'util/redux-form/util/validate'
import {
  hasMaxBulletCountDraftJS,
  hasValidServiceClasses,
  hasValueDraftJSWithGDCheck,
  isRequired,
  isRequiredDraftJs,
  isNotEmpty,
  isEqual,
  hasMinLengthDraftJS,
  hasMaxItemCountWithGD,
  serviceProducerValidation,
  arrayValidator
} from 'util/redux-form/validators'
import { validationMessageTypes } from 'util/redux-form/validators/types'
import {
  getService,
  getAdditionalQualityCheckData,
  getServiceByGeneralDescription,
  canArchiveAstiEntity
} from 'Routes/Service/selectors'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { EntitySchemas } from 'schemas'
import { messages as commonFormMessages } from 'Routes/messages'
import { customValidationMessages } from 'util/redux-form/validators/messages'
import {
  formTypesEnum,
  formActionsTypesEnum,
  DRAFTJS_MIN_LENGTH,
  DRAFTJS_MAX_BULLET_COUNT,
  SERVICE_CLASSES_MAX_COUNT,
  ONTOLOGY_TERMS_MAX_COUNT
} from 'enums'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { getGDPublishedLanguages } from 'Routes/Service/components/ServiceComponents/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { Map } from 'immutable'
import cx from 'classnames'
import styles from './styles.scss'
import { qualityEntityCheck, qualityEntityCheckIfNotRun, qualityCheckCancelChanges } from 'actions/qualityAgent'

export const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.Services.AddService.Header.Title',
    defaultMessage: 'Lisää uusi palvelu'
  },
  formTitle: {
    id: 'Containers.Services.AddService.Step1.Header.Title',
    defaultMessage: 'Vaihe 1/4: Palvelun perustiedot'
  },
  formTitle2: {
    id: 'Containers.Services.AddService.Step2.Header.Title',
    defaultMessage: 'Vaihe 2/4: Luokittelu ja ontologiakäsitteet'
  },
  formTitle2InfoText: {
    id: 'Routes.Service.ServiceForm.Section2.InfoText',
    defaultMessage: 'Tämän osion tiedot eivät näy loppukäyttäjille.'
  },
  formTitle3: {
    id: 'Containers.Services.AddService.Step3.Header.Title',
    defaultMessage: 'Vaihe 3/4: Palvelun tuottaminen ja alueelliset tiedot'
  }
})

const handleNewLanguageResponse = (newLanguageCode, { getState, dispatch }) => {
  const state = getState()
  const getServiceFormSelector = formValueSelector(formTypesEnum.SERVICEFORM)

  const serviceName = getServiceFormSelector(state, 'name') || Map()
  const generalDescriptionId = getServiceFormSelector(state, 'generalDescriptionId')
  const generalDescription = generalDescriptionId && EntitySelectors.generalDescriptions.getEntity(state,
    { id: generalDescriptionId })
  const generalDescriptionName = generalDescription && generalDescription.get('name') || Map()
  const publishedGDLanguages = getGDPublishedLanguages(state, { formName:formTypesEnum.SERVICEFORM })

  let someNameIsOverwritenWithGD = false
  if (generalDescriptionName && generalDescriptionName.size > 0) {
    serviceName.forEach((name, langCode) => {
      if (publishedGDLanguages.contains(langCode)) {
        someNameIsOverwritenWithGD = name === generalDescriptionName.get(langCode)
        if (someNameIsOverwritenWithGD) return
      }
    })

    if (someNameIsOverwritenWithGD && publishedGDLanguages.contains(newLanguageCode)) {
      let nameGD = generalDescriptionName.get(newLanguageCode)
      let newServiceNames = serviceName.set(newLanguageCode, nameGD)

      dispatch(
        change(
          formTypesEnum.SERVICEFORM,
          'name',
          newServiceNames
        )
      )
    }
  }
}

const handleActionCancel = (action, actionCallback, { getState, dispatch }) => {
  const state = getState()
  const canArchive = canArchiveAstiEntity(state)
  switch (action) {
    case formActionsTypesEnum.ARCHIVEENTITY:
    case formActionsTypesEnum.ARCHIVELANGUAGE:
      canArchive
        ? actionCallback(action)
        : dispatch(mergeInUIState({
          key: `${formTypesEnum.SERVICEFORM}${action}CancelDialog`,
          value: {
            isOpen: true,
            action
          }
        }))
      break
    default:
      actionCallback(action)
  }
}

const basicPublishValidators = [
  { path: 'fundingType', validate: isRequired('fundingType') },
  {
    path: 'name',
    validate: isEqual('shortDescription', formTypesEnum.SERVICEFORM)('name'),
    type: validationMessageTypes.asErrorVisible
  },
  {
    path: 'description',
    validate: isRequiredDraftJs('description')
  },
  {
    path: 'description',
    validate: hasMaxBulletCountDraftJS(DRAFTJS_MAX_BULLET_COUNT)('description'),
    type: validationMessageTypes.visible
  },
  {
    path: 'description',
    validate: hasMinLengthDraftJS(DRAFTJS_MIN_LENGTH)('description'),
    type: validationMessageTypes.visible
  },
  { path: 'shortDescription', validate: isRequired('shortDescription') },
  {
    path: 'shortDescription',
    validate: isEqual('name', formTypesEnum.SERVICEFORM)('shortDescription'),
    type: validationMessageTypes.asErrorVisible
  },
  {
    path: 'userInstruction',
    validate: hasValueDraftJSWithGDCheck('userInstruction')('userInstruction'),
    type: validationMessageTypes.visible
  },
  { path: 'languages', validate: isNotEmpty('languages') }
]
const classificationPublishValidators = [
  { path: 'targetGroups', validate: isNotEmpty('targetGroups') },
  { path: 'serviceClasses', validate: isNotEmpty('serviceClasses') },
  {
    path: 'serviceClasses',
    validate: hasMaxItemCountWithGD(
      SERVICE_CLASSES_MAX_COUNT,
      'serviceClasses',
      customValidationMessages.serviceClassItemMaxCountReached
    )('serviceClasses'),
    type: validationMessageTypes.visible
  },
  {
    path: 'serviceClasses',
    validate: hasValidServiceClasses('serviceClasses')('serviceClasses'),
    type: validationMessageTypes.visible
  },
  { path: 'ontologyTerms', validate: isNotEmpty('ontologyTerms') },
  {
    path: 'ontologyTerms',
    validate: hasMaxItemCountWithGD(
      ONTOLOGY_TERMS_MAX_COUNT,
      'ontologyTerms',
      customValidationMessages.ontologyTermItemMaxCountReached
    )('ontologyTerms'),
    type: validationMessageTypes.visible
  }
]
const provisionMethodsAndProviderValidators = [
  // {
  //   path: 'serviceProducers',
  //   validate: arrayValidator(
  //     serviceProducerValidation('serviceProducers')
  //   )
  // },
  {
    path: 'serviceProducers',
    validate: isNotEmpty('serviceProducers')
  }
]
const publishValidators = [
  ...basicPublishValidators,
  ...classificationPublishValidators,
  ...provisionMethodsAndProviderValidators
]
const fieldsRequiredForSave = [
  'name',
  'organization'
]
const basicPublishFields = basicPublishValidators
  .map(validator => validator.path)
const classificationPublishFields = classificationPublishValidators
  .map(validator => validator.path)

class ServiceForm extends Component {
  render () {
    const {
      handleSubmit,
      form,
      intl: { formatMessage },
      isCompareMode,
      inTranslation
    } = this.props
    const formClass = cx(
      styles.form,
      {
        [styles.compareMode]: isCompareMode
      }
    )
    return (
      <form onSubmit={handleSubmit} className={formClass}>
        <LanguageComparisonSelect />
        <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'serviceAccordion1'} >
          <ReduxAccordion.Title
            publishFields={basicPublishFields}
            saveFields={fieldsRequiredForSave}
            title={formatMessage(messages.formTitle)}
          />
          <div className='row'>
            <div className='col-lg-12'>
              <ValidationMessages form={form} top />
            </div>
          </div>
          <ReduxAccordion.Content>
            <ServiceBasic />
          </ReduxAccordion.Content>
        </ReduxAccordion>
        <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'serviceAccordion2'}>
          <ReduxAccordion.Title
            publishFields={classificationPublishFields}
            title={formatMessage(messages.formTitle2)}
            helpText={formatMessage(messages.formTitle2InfoText)} />
          <ReduxAccordion.Content>
            <ServiceClassificationAndKeywords />
          </ReduxAccordion.Content>
        </ReduxAccordion>
        <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'serviceAccordion3'}>
          <ReduxAccordion.Title
            publishFields={['serviceProducers']}
            title={formatMessage(messages.formTitle3)}
          />
          <ReduxAccordion.Content>
            <ServiceProducerInfo />
          </ReduxAccordion.Content>
        </ReduxAccordion>
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} />
          </div>
        </div>
      </form>
    )
  }
}
ServiceForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  isCompareMode: PropTypes.bool,
  inTranslation: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'service/SaveService',
  transformers: [
    languagesAvailabilitiesTransformer,
    serviceBasicTransformer
  ],
  schema: EntitySchemas.SERVICE
})

const entityType = 'service'

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getService)
  dispatch(qualityEntityCheck(result, entityType))
}

const onEdit = ({ formName }) => store => {
  store.dispatch(qualityEntityCheckIfNotRun({ formName }, entityType))
}

const onCancel = ({ formName }) => store => {
  store.dispatch(qualityCheckCancelChanges({ formName }, entityType))
}

const onSubmitFail = (...args) => console.log(args)

const getIsLoading = getIsFormLoading(formTypesEnum.SERVICEFORM)

const warn = validate(publishValidators)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.services.getEntityIsFetching(state) || getIsLoading(state)
      const gdId = ownProps.location && ownProps.location.state && ownProps.location.state.gd
      const initialValues = !ownProps.id && gdId
        ? getServiceByGeneralDescription(state, { gdId, ...ownProps })
        : getService(state, ownProps)
      return {
        isLoading,
        initialValues,
        copyId: ownProps.copyId,
        isInReview: getShowReviewBar(state)
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.SERVICEFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess,
    warn,
    shouldWarn: () => true
  }),
  withBubbling({
    framed: true,
    padded: true
  }),
  withAutomaticSave({
    draftJSFields: [
      'description',
      'conditionOfServiceUsage',
      'userInstruction'
    ]
  }),
  withEntityNotification,
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  branch(({ isInReview }) => !isInReview, withConnectionStep),
  withEntityButtons({
    formNameToSubmit: formTypesEnum.SERVICEFORM,
    onEdit,
    onCancel
  }),
  withEntityHeader({
    entityId: null,
    handleNewLanguageResponse: handleNewLanguageResponse,
    handleActionCancel: handleActionCancel
  }),
  withFormStates,
  withPublishingDialog(),
  withPreviewDialog,
  withQualityAgentProvider({
    entityPrefix: 'service',
    dataSelector: getAdditionalQualityCheckData
  })
)(ServiceForm)

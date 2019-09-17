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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { defineMessages } from 'util/react-intl'
import { getIsInTranslation } from 'selectors/entities/entities'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import cx from 'classnames'
import styles from './styles.scss'

export const messages = defineMessages({
  translationInProgress: {
    id: 'Util.ReduxForm.HOC.WithTranslationLock.Title',
    defaultMessage: 'Translation in progress'
  }
})

const withTranslationLock = ({ fieldType = '' } = {}) => WrappedComponent => {
  const InnerComponent = ({
    isLocked,
    disabled,
    ...rest
  }) => {
    const translationLockInfoClass = cx(
      styles.translationLockInfo,
      fieldType && styles[fieldType]
    )
    return (
      <div>
        <WrappedComponent
          disabled={isLocked || disabled}
          {...rest}
        />
        {isLocked &&
          <div className={translationLockInfoClass}>{rest.intl.formatMessage(messages.translationInProgress)}</div>
        }
      </div>
    )
  }
  InnerComponent.propTypes = {
    isLocked: PropTypes.bool,
    disabled: PropTypes.bool
  }
  return compose(
    injectFormName,
    withLanguageKey,
    connect(
      (state, { isReadOnly, languageKey, noTranslationLock, formName, compare }) => {
        const language = getContentLanguageCode(state, { formName, languageKey }) || 'fi'
        const comparisionLanguageCode = getSelectedComparisionLanguageCode(state)
        const languageCode = compare && comparisionLanguageCode || language
        const progress = getIsInTranslation(state, { languageCode })
        return {
          isLocked: (!noTranslationLock && !isReadOnly && progress) || false
        }
      }
    )
  )(InnerComponent)
}

export default withTranslationLock

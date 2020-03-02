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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import styles from './styles.scss'
import { Label } from 'sema-ui-components'
import { getHasAstiInformation } from './selectors'
import NoDataLabel from 'appComponents/NoDataLabel'
import AstiInformation from 'appComponents/ConnectionsStep/AdditionalInformation/AstiInformation'

const messages = defineMessages({
  astiTitle: {
    id: 'AppComponents.ConnectionStep.ConnectionsForm.AdditionalInformation.Tabs.ASTI.Title',
    defaultMessage: 'Asti-palvelutasot'
  }
})

const ConnectionOpeningHours = ({
  intl: { formatMessage },
  label,
  field,
  isVisible,
  ...rest
}) => {
  return (
    <div className={styles.previewBlock}>
      <Label className={styles.previewHeading} labelText={label || formatMessage(messages.astiTitle)} />
      <div className={styles.previewItem}>
        {isVisible && <AstiInformation field={field} isReadOnly hideInfo /> || <NoDataLabel />}
      </div>
    </div>
  )
}
ConnectionOpeningHours.propTypes = {
  intl: intlShape,
  label: PropTypes.string,
  field: PropTypes.string,
  isVisible: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withLanguageKey,
  connect((state, { formName, field, languageKey }) => ({
    isVisible: getHasAstiInformation(state, { formName, field, languageKey })
  }))
)(ConnectionOpeningHours)

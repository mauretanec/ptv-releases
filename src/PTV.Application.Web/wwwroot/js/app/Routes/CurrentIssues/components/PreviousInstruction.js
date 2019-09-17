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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import PropTypes from 'prop-types'
import moment from 'moment'
import { SecurityRead } from 'appComponents/Security'
import { Label } from 'sema-ui-components'
import {
  Description
} from 'util/redux-form/fields'
import { getPreviousFormInstruction } from 'Routes/CurrentIssues/selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const messages = defineMessages({
  previousIssuesTexEditorLabel: {
    id: 'CurrentIssues.Header.PreviousIssues.Label',
    defaultMessage: 'Tiedotteet - previous'
  },
  previousIssueModified: {
    id: 'CurrentIssues.Header.PreviousIssues.Modified.Label',
    defaultMessage: 'Muokattu: {modified}'
  },
  previousIssueModifiedBy: {
    id: 'CurrentIssues.Header.PreviousIssues.ModifiedBy.Label',
    defaultMessage: 'Muokkaaja: {modifiedBy}'
  }
})

const PreviousInstruction = ({
  info,
  intl: { formatMessage } }) => {
  return (
    <SecurityRead domain='previousIssues'>
      <div className='col-xs-12'>
        <Description
          label={formatMessage(messages.previousIssuesTexEditorLabel)}
          name={'previousData'}
          isReadOnly
          isLocalized={false}
        />
        <Label
          infoLabel
          labelText={formatMessage(messages.previousIssueModified,
            { modified: moment(info.get('modified')).format('DD.MM.YYYY HH:mm') }) + ' ' +
        formatMessage(messages.previousIssueModifiedBy, { modifiedBy: info.get('modifiedBy') })} />
      </div>
    </SecurityRead>
  )
}

PreviousInstruction.propTypes = {
  intl: intlShape.isRequired,
  info: PropTypes.object
}

export default compose(
  injectFormName,
  connect((state, ownProps) => ({
    info: getPreviousFormInstruction(state, ownProps)
  })
  ),
  injectIntl
)(PreviousInstruction)

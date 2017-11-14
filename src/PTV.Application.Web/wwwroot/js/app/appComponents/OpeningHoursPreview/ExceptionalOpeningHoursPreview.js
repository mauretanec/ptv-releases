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
// import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { formValueSelector } from 'redux-form/immutable'
import { List } from 'immutable'
import moment from 'moment'
import { Label } from 'sema-ui-components'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import styles from './styles.scss'

const messages = defineMessages({
  closeMessage: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.ClosedMessage',
    defaultMessage: 'Suljettu'
  },
  openMessage: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.OpenMessage',
    defaultMessage: 'Avoinna'
  }
})

const ExceptionalOpeningHoursPreview = ({
  exceptionalOpeningHours,
  intl: { formatMessage }
}) => {
  return (
    exceptionalOpeningHours && exceptionalOpeningHours.size > 0 &&
    <div className={styles.previewBlock}>
      {exceptionalOpeningHours
      .filter(x => typeof x !== 'undefined')
      .map((exceptionalOpeningHour, index) => {
        const isClosedForPeriod =
          exceptionalOpeningHour.get('closedForPeriod')
        const title = exceptionalOpeningHour.get('title')
        const openCloseMessage = isClosedForPeriod
          ? formatMessage(messages.closeMessage)
          : formatMessage(messages.openMessage)
        const timeFrom =
          exceptionalOpeningHour.has('timeFrom') &&
          exceptionalOpeningHour.get('timeFrom') !== null &&
          <span>{moment.utc(exceptionalOpeningHour.get('timeFrom')).format('HH:mm')}</span>
        const timeTo =
          exceptionalOpeningHour.has('timeTo') &&
          exceptionalOpeningHour.get('timeTo') !== null &&
          <span>{moment.utc(exceptionalOpeningHour.get('timeTo')).format('HH:mm')}</span>
        const dateFrom =
          exceptionalOpeningHour.has('dateFrom') &&
          exceptionalOpeningHour.get('dateFrom') !== null &&
          exceptionalOpeningHour.get('dateFrom')
        const dateFromFormatted = moment(dateFrom).isValid() && moment(dateFrom).format('DD.MM.YYYY')
        const dateTo =
          exceptionalOpeningHour.has('dateTo') &&
          exceptionalOpeningHour.get('dateTo') !== null &&
          exceptionalOpeningHour.get('dateTo')
        const dateToFormatted = moment(dateTo).isValid() && moment(dateTo).format('DD.MM.YYYY')
        return (
          <div className={styles.previewBlockItem}>
            {title && <Label labelText={title} />}
            <div key={index} className={styles.record}>
              <div>{openCloseMessage}</div>
              <div className={styles.from}>
                <span>{dateFromFormatted}</span>
                <span>{timeFrom}</span>
              </div>
              <div>{(dateTo || timeTo) && '-'}</div>
              <div className={styles.to}>
                <span>{dateToFormatted}</span>
                <span>{timeTo}</span>
              </div>
            </div>
          </div>
        )
      })}
    </div>
  )
}
ExceptionalOpeningHoursPreview.propTypes = {
  exceptionalOpeningHours: ImmutablePropTypes.list.isRequired,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(
    (state, { formName }) => {
      const getFormValues = formValueSelector(formName)
      return {
        exceptionalOpeningHours: getFormValues(state, 'openingHours.exceptionalOpeningHours') || List()
      }
    }
  )
)(ExceptionalOpeningHoursPreview)

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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { ChannelAddressSearchFrom } from 'appComponents/ChannelAddressSearch'
import styles from './styles.scss'
import { Accordion } from 'appComponents/Accordion'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { connect } from 'react-redux'
import { getContentLanguageCode } from 'selectors/selections'

export const messages = defineMessages({
  addressSearchTitle: {
    id: 'Routes.Channels.ServiceLocation.components.AddressSearch.Title',
    defaultMessage: 'Hae samasta katuosoitteesta löytyvät muut palvelupaikat'
  },
  addressSearchTooltip: {
    id: 'Routes.Channels.ServiceLocation.components.AddressSearch.Tooltip',
    defaultMessage: 'Hae samasta katuosoitteesta löytyvät muut palvelupaikat'
  }
})

const withAddressSearch = InnerComponent => {
  const WrappedComponent = ({
    isNew,
    hasContentLanguage,
    ...rest
  }) => (
    <div>
      {isNew && (
        <div className={styles.addressSearch}>
          <Accordion activeIndex={-1}>
            <Accordion.Title
              validate
              title={rest.intl.formatMessage(messages.addressSearchTitle)}
              tooltip={rest.intl.formatMessage(messages.addressSearchTooltip)}
              withSearch
              inActive={!hasContentLanguage}
            />
            <Accordion.Content>
              <div className='form-row'>
                <ChannelAddressSearchFrom />
              </div>
            </Accordion.Content>
          </Accordion>
        </div>
      )}
      <InnerComponent {...rest} />
    </div>
  )

  WrappedComponent.propTypes = {
    intl: intlShape.isRequired,
    isNew: PropTypes.bool.isRequired,
    hasContentLanguage: PropTypes.string
  }
  return compose(
    injectIntl,
    connect(state => ({
      isNew: !getSelectedEntityId(state),
      hasContentLanguage: getContentLanguageCode(state)
    }))
  )(WrappedComponent)
}

export default withAddressSearch

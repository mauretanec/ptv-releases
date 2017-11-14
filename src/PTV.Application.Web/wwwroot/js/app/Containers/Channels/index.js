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
import WebPageContainer from './WebPage/Pages'
import ElectronicContainer from './Electronic/Pages'
import ServiceLocationContainer from './ServiceLocation/Pages'
import PhoneContainer from './Phone/Pages'
import PrintableFormContainer from './PrintableForm/Pages'

const ChannelsContainer = props => {
  const renderContainer = props => {
    switch (props.params.view) {
      case 'manage':
        switch (props.params.id) {
          case 'webPage':
            return <WebPageContainer {...props} />
          case 'eChannel':
            return <ElectronicContainer {...props} />
          case 'phone':
            return <PhoneContainer {...props} />
          case 'printableForm':
            return <PrintableFormContainer {...props} />
          case 'serviceLocation':
            return <ServiceLocationContainer {...props} />
          default:
            return null
        }
      default:
        return null
    }
  }

  renderContainer.propTypes = {
    params: PropTypes.object
  }

  return renderContainer(props)
}

export default ChannelsContainer


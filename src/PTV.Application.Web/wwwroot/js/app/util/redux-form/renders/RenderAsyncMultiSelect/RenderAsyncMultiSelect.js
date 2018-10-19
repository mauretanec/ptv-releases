/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react'
import PropTypes from 'prop-types'
import callApi from 'util/callApi'
import { Select } from 'sema-ui-components'
import { noop } from 'lodash'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import Popup from 'appComponents/Popup'
import { PTVIcon } from 'Components'
import cx from 'classnames'
import styles from './styles.scss'
import injectDefaultTexts from '../RenderSelect/injectDefaultTexts'
import { getEntitiesForIdsJS } from 'selectors/base'
import { OrderedSet } from 'immutable'

const RenderAsyncMultiSelect = ({
  input,
  url,
  formatResponse,
  onOptionsLoaded,
  defaultOptions,
  getQueryData,
  inlineLabel,
  label,
  tooltip,
  placeholder,
  onChangeObject,
  minLength = 2,
  complete = true,
  error,
  errorMessage,
  options,
  ...rest
}) => {
  const loadOptions = async query => {
    if (!query || query.length < minLength) {
      return defaultOptions && (!query || query.length === 0) && {
        options: formatResponse
          ? formatResponse(defaultOptions)
          : defaultOptions.map(({ value, label }) => ({ value, label })),
        complete
      } || null
    }
    try {
      const { data } = await callApi(url, getQueryData && getQueryData(query, rest) || query)
      if (onOptionsLoaded) {
        onOptionsLoaded(data)
      }
      return {
        options: formatResponse
          ? formatResponse(data)
          : data.map(({ value, label }) => ({ value, label })),
        complete
      }
    } catch (err) {
      console.log('Async select err:\n', err)
    }
  }
  const labelChildren = () => (
    tooltip &&
    <Popup
      trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
      content={tooltip}
    />
  )
  const wrapClass = cx(
    styles.autosizeOff,
    styles.multiFix
  )
  return (
    <div className={wrapClass}>
      <Select.Async
        searchable
        multi
        ignoreAccents={false}
        label={label}
        labelChildren={rest.labelPosition !== 'inside' && labelChildren()}
        placeholder={placeholder}
        loadOptions={loadOptions}
        {...input}
        value={options && options.size && input.value.size && getEntitiesForIdsJS(options, input.value) || input.value && input.value.size && input.value}
        onChange={option => {
          onChangeObject && onChangeObject(option)
          input.onChange(option && OrderedSet(option.map(x => x.value) || OrderedSet())) // This is just awful tbh
        }}
        size='full'
        clearable
        {...rest}
        onBlur={noop} // onBlurResetsInput doesn't work react-select #751
        onBlurResetsInput={false}
        error={error}
        errorMessage={errorMessage}
      />
    </div>
  )
}
RenderAsyncMultiSelect.propTypes = {
  inlineLabel: PropTypes.string
}

RenderAsyncMultiSelect.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor,
  injectDefaultTexts
)(RenderAsyncMultiSelect)

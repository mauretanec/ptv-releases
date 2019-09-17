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
import { defineMessages } from 'util/react-intl'
import { isValid } from './isValid'
import { getOidParser } from 'selectors/common'

const message = defineMessages({
  isOid: {
    id: 'Components.Validators.IsOid.Message',
    defaultMessage: 'OID should be in following format (1.2.246.10.10000549.10.0, 1.2.246.10.10065385.10.105027)'
  }
})

const isOid = (oid, pattern) => oid && !oid.match(pattern)

export default isValid(({ value, formProps }) => {
  let pattern
  formProps.dispatch(({ getState }) => {
    const state = getState()
    pattern = getOidParser(state)
  })
  return isOid(value, pattern)
}, {
  validationMessage: message.isOid
})

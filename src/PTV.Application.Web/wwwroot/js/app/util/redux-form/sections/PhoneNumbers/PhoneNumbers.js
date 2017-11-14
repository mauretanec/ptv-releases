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
import { FormSection } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { withFormStates } from 'util/redux-form/HOC'
import {
  ChargeType,
  DialCode,
  IsLocalNumber,
  PhoneCostDescription,
  PhoneNumber,
  PhoneNumberInfo
} from 'util/redux-form/fields'
import {
  getIsLocalNumberSelectedForIndex,
  getIsChargeTypeOtherSelectedForIndex
} from './selectors'

class PhoneNumbers extends FormSection {
  static defaultProps = {
    name: 'phoneNumbers'
  }
  render () {
    const {
      dialCodeProps = {},
      phoneNumberProps = {},
      phoneNumberInfoProps = {},
      localServiceNumberLabel,
      chargeTypeProps = {},
      phoneCostDescriptionProps = {},
      isCompareMode,
      compare,
      isLocalNumberSelected,
      isChargeTypeOtherSelected,
      splitView
    } = this.props
    return (
      <div>
        {!isCompareMode && !splitView
          ? <div>
            <div className='form-row'>
              <div className='row'>
                <div className='col-lg-6'>
                  <div className='row'>
                    <div className='col-24 mb-2'>
                      <DialCode
                        isCompareMode={isCompareMode}
                        disabled={isLocalNumberSelected}
                        compare={compare}
                        {...dialCodeProps}
                      />
                    </div>
                    <div className='col-24'>
                      <IsLocalNumber
                        isCompareMode={isCompareMode}
                        label={localServiceNumberLabel} />
                    </div>
                  </div>
                </div>
                <div className='col-lg-6'>
                  <PhoneNumber
                    size='full'
                    isCompareMode={isCompareMode}
                    {...phoneNumberProps}
                  />
                </div>
              </div>
              <div className='form-row'>
                <div className='row'>
                  <div className='col-lg-12'>
                    <PhoneNumberInfo
                      isCompareMode={isCompareMode}
                      {...phoneNumberInfoProps}
                    />
                  </div>
                </div>
              </div>
            </div>
            <div className='form-row'>
              <div className='row'>
                <div className='col-lg-6'>
                  <ChargeType
                    isCompareMode={isCompareMode}
                    compare={compare}
                    {...chargeTypeProps} />
                </div>
              </div>
            </div>
            <div className='form-row'>
              <div className='row'>
                <div className='col-lg-12'>
                  <PhoneCostDescription
                    size='full'
                    counter
                    maxLength={150}
                    isCompareMode={isCompareMode}
                    {...phoneCostDescriptionProps}
                    required={isChargeTypeOtherSelected} />
                </div>
              </div>
            </div>
          </div>
        : <div>
          <div className='form-row'>
            <DialCode
              size='w240'
              disabled={isLocalNumberSelected}
              isCompareMode={isCompareMode}
              compare={compare}
              {...dialCodeProps}
            />
          </div>
          <div className='form-row'>
            <IsLocalNumber isCompareMode={isCompareMode} label={localServiceNumberLabel} />
          </div>
          <div className='form-row'>
            <PhoneNumber
              size='full'
              isCompareMode={isCompareMode}
              {...phoneNumberProps}
            />
          </div>
          <div className='form-row'>
            <PhoneNumberInfo
              isCompareMode={isCompareMode}
              {...phoneNumberInfoProps}
            />
          </div>
          <div className='form-row'>
            <ChargeType
              isCompareMode={isCompareMode}
              size='w240'
              compare={compare}
              {...chargeTypeProps} />
          </div>
          <div className='form-row'>
            <PhoneCostDescription
              size='full'
              counter
              maxLength={150}
              isCompareMode={isCompareMode}
              {...phoneCostDescriptionProps}
              required={isChargeTypeOtherSelected} />
          </div>
        </div>
        }
        {this.props.children}
      </div>
    )
  }
}

export default compose(
  withFormStates,
  connect((state, ownProps) => ({
    isLocalNumberSelected: getIsLocalNumberSelectedForIndex(state, ownProps),
    isChargeTypeOtherSelected: getIsChargeTypeOtherSelectedForIndex(state, ownProps)
  })),
)(PhoneNumbers)

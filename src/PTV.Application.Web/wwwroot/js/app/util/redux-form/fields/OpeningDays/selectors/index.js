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
import { createSelector } from 'reselect'
import { Map, List } from 'immutable'
import { getFormValues } from 'redux-form/immutable'
import { daysOfTheWeek } from '../OpeningDays'
import _ from 'lodash'

const getOpeningDays = (formName, index) => createSelector(
  getFormValues(formName),
  formValues => formValues.getIn(['openingHours', 'normalOpeningHours', index, 'dailyOpeningHours']) || Map()
)
export const getOpeningDaysNames = (formName, index) => createSelector(
  getOpeningDays(formName, index),
  openingDays => {
    const intervalNames = daysOfTheWeek.reduce((acc, dayName) => {
      const intervals = openingDays.getIn([dayName, 'intervals'])
      if (intervals && intervals.size !== 0) {
        intervals.forEach((interval, index) => {
          acc = acc
            .push(`dailyOpeningHours.${dayName}.intervals[${index}].from`)
            .push(`dailyOpeningHours.${dayName}.intervals[${index}].to`)
        })
      }
      return acc
    }, List())
    const result = _.uniq([
      'isOpenNonStop',
      ...daysOfTheWeek.map(dayName => `dailyOpeningHours.${dayName}`),
      ...daysOfTheWeek.map(dayName => `dailyOpeningHours.${dayName}.active`),
      ...daysOfTheWeek.map(dayName => `dailyOpeningHours.${dayName}.intervals[0].from`),
      ...daysOfTheWeek.map(dayName => `dailyOpeningHours.${dayName}.intervals[0].to`),
      ...intervalNames.toJS()
    ])
    return result
  }
)

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
import { Field, formValues, change } from 'redux-form/immutable'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import withPath from 'util/redux-form/HOC/withPath'
import shortId from 'shortid'
import withQualityAgent from 'util/redux-form/HOC/withQualityAgent'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.Common.OpeningHours.AlternativeTitle',
    defaultMessage: 'Otsikko'
  },
  placeholder: {
    id: 'ReduxForm.Fields.Title.Placeholder',
    defaultMessage: 'Anna tarvittaessa otsikko'
  }
})

const Title = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='title'
    labelPosition='top'
    component={RenderTextField}
    label={formatMessage(messages.label)}
    placeholder={formatMessage(messages.placeholder)}
    {...rest}
  />
)
Title.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  asLocalizable,
  withPath,
  formValues('order'),
  withQualityAgent({
    key: ({ contentLanguage = 'fi', order, collectionPrefix, postFix = 'title' }) => `${collectionPrefix}.${contentLanguage}.${order}.${postFix}`,
    customAction: ({ formName, order, path }) => (store) => {
      typeof order === 'undefined' && store.dispatch(change(formName, `${path}.order`, shortId.generate()))
    }
  })
)(Title)

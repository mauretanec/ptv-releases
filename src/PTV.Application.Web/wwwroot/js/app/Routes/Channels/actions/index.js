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
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'

export const loadChannelHeader = id => apiCall3({
  keys: ['channels', 'header'],
  payload: {
    endpoint: 'channel/GetChannelHeader',
    data: { id }
  },
  schemas: EntitySchemas.CHANNEL
})

export const createGetChannelAction = (endpointAction, form) => (data, successNextAction) => {
  return apiCall3({
    keys: ['channels', 'load'],
    payload: {
      endpoint: `channel/${endpointAction}`,
      data
    },
    schemas: EntitySchemas.CHANNEL,
    formName: form,
    successNextAction
  })
}

export const loadConnections = (newId, id) => {
  if (newId === id) {
    return apiCall3({
      keys: ['channels', 'relations'],
      payload: {
        endpoint:'channel/GetRelations',
        data: { id }
      },
      schemas: EntitySchemas.CHANNEL
    })
  }
}

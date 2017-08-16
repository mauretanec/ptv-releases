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
import { Map } from 'immutable'
import isClientRender from './is-client-render'

export const playbackFlag = 'REDUX_BUG_REPORTER_PLAYBACK'

export const overloadStoreActionType = 'REDUX_BUG_REPORTER_OVERLOAD_STORE'
export const overloadStore = payload => ({
  type: overloadStoreActionType,
  payload
})

export const initializePlaybackActionType = 'REDUX_BUG_REPORTER_INITIALIZE_PLAYBACK'
export const initializePlayback = () => ({
  type: initializePlaybackActionType
})

export const finishPlaybackActionType = 'REDUX_BUG_REPORTER_FINISH_PLAYBACK'
export const finishPlayback = payload => ({
  type: finishPlaybackActionType
})

let storeEnhancer = f => f
if (isClientRender()) {
  storeEnhancer = (createStore) => (originalReducer, initialState, enhancer) => {
    let playbackEnabled = false
    // Handle the overloading in the reducer here
    let reducer = function (state, action = {}) {
      if (action.type === overloadStoreActionType) {
        console.warn('Overriding the store. You should only be doing this if you are using the bug reporter')
        return action.payload
      } else if (action.type === initializePlaybackActionType) {
        // starting playback
        playbackEnabled = true
        return state
      } else if (action.type === finishPlaybackActionType) {
        // stopping playback
        playbackEnabled = false
        return state
      }
      // Log the action
      if (isClientRender() && !playbackEnabled) {
        const actions = middlewareData.getActions()
        // If this is the first action, log the initial state
        if (actions.length === 0) {
          middlewareData.setBugReporterInitialState(state)
        }
        middlewareData.addAction(action)
      }
      // Remove the playback flag from the payload
      if (action[playbackFlag]) {
        delete action[playbackFlag]
      }
      return originalReducer(...arguments)
    }
    const store = createStore(reducer, initialState, enhancer)
    const { dispatch: origDispatch } = store
    middlewareData.clearActions()
    middlewareData.setBugReporterInitialState(Map())

    // wrap around dispatch disable all non-playback actions during playback
    const dispatch = (action, ...rest) => {
      // Allow overload and finishPlayback actions
      if (action && action.type &&
        (action.type === overloadStoreActionType || action.type === finishPlaybackActionType)) {
        return origDispatch(action, ...rest)
      }
      if (playbackEnabled && !action[playbackFlag]) {
        // ignore the action
        return
      }
      return origDispatch(action, ...rest)
    }

    return {
      ...store,
      dispatch
    }
  }
}

export let middlewareData = {
  actions: [],
  bugReporterInitialState: Map(),
  addAction: function (action) {
    this.actions.push(action)
  },
  clearActions: function () {
    this.actions = []
  },
  getActions: function () {
    return this.actions
  },
  setBugReporterInitialState: function (state) {
    this.bugReporterInitialState = state
  },
  getBugReporterInitialState: function () {
    return this.bugReporterInitialState
  }
}

export default storeEnhancer

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
import ReactDOM from 'react-dom';
import { fromJS } from 'immutable';
 
if (!window.location.origin) {
  window.location.origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port: '');
}

const windowLocation = window.location.origin;

const APP_URL = windowLocation;

export function getAppUrl()
{
	return APP_URL;//.replace('://', '://uiapi.');
}

export function scrollTo(ref){	
	if(ref){
		var node = ReactDOM.findDOMNode(ref);
		window.scrollTo(0, node.offsetTop);
	}	   
}

export const loadState = () => {
	try {
		const serializedState = localStorage.getItem('state');
		if ( serializedState === null ) {
			return undefined;
		}

		const json = fromJS(JSON.parse(serializedState));
		return json;
	}
	catch (err) {
		return undefined;
	}
}

export const saveState = state => {
	try {
		const serializedState = JSON.stringify(state);
		localStorage.setItem('state', serializedState);
	}
	catch (err) {
		console.error(err);
	}
}

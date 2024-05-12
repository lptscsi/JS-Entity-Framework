# JRIAppAngular
Testing the usage of JRIApp DB part with Angular (using the dataService + data binding on the client side)

JRIApp DB supports DbContext and entities on the client side. The Bind Directive supports binding form controls to the entity properties.
Optinally it supports using converters for automatic two way update.\
It works for handling editing changes for angular forms and angular UI controls (for example, angular primeng tables)\
It resembles working with RIA services in the Silverlight application, albeit it works for HTML on the client side.\
The good part, you can make changes and reject or submit them in one transaction from the client side.



_At first it is needed to restore client packages_

  npm install

_then build jriapp-lib using angular build command_

  ng build jriapp-lib


_then build the Application_

  ng build --watch

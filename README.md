# JRIAppAngular
Testing the usage of JRIApp DB part with Angular (using the dataService + data binding on the client side)

JRIApp DB supports DbContext and entities on the client side. The Bind Directive supports binding form controls to the entity properties.
Optinally it supports using converters for automatic two way update.


_At first it is needed to restore client packages_

  npm install

_then build jriapp-lib using angular build command_

  ng build jriapp-lib


_then build the Application_

  ng build --watch

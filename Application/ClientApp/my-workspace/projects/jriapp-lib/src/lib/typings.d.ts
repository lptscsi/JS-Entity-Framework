import { Dayjs, ConfigType } from './dayjs/dayjs';

declare global {
  var jriapp_config: any;
  
  const dayjs: {
    // Basic initialization
    (date?: ConfigType): Dayjs;
    // Strict parsing initialization signature
    (date: string, format: string | string[], strict: boolean): Dayjs;
    (date: string, format: string | string[], locale: string, strict: boolean): Dayjs;
    
    extend(plugin: any, option?: any): any;
  };
  
  // Expose the global window plugin object loaded by angular.json
  const dayjs_plugin_customParseFormat: any;
}

export {};
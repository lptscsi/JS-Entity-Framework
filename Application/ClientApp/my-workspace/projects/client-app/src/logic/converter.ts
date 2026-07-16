import { Utils } from 'jriapp-lib';

export interface IConverter {
  convertToSource(val: any, param: any): any;
  convertToTarget(val: any, param: any): any;
}

const utils = Utils, { isNt, isNumber, isDate, isString } = utils.check, { format, stripNonNumeric, formatNumber } = utils.str,
    { round } = utils.core, { strToDate, dateToStr } = utils.dates;

export const NUM_CONV = { None: 0, Integer: 1, Decimal: 2, Float: 3, SmallInt: 4 };

const dateFormat = 'YYYY-MM-DD', decimalPoint = '.', thousandSep = ' ', decPrecision = 2;

export class BaseConverter implements IConverter {
    convertToSource(val: any, _param: any): any {
        return val;
    }
    convertToTarget(val: any, _param: any): any {
        return (isNt(val)) ? null : val;
    }
}
export const baseConverter = new BaseConverter();

export class DateConverter implements IConverter {
    convertToSource(val: any, _param: any): Date {
        if (!val) {
            return null;
        }
      return dateTimeConverter.convertToSource(val, dateFormat);
    }
    convertToTarget(val: any, _param: any): string {
        if (isNt(val)) {
            return "";
        }
        return dateTimeConverter.convertToTarget(val, dateFormat);
    }
    toString() {
        return "DateConverter";
    }
}
export const dateConverter = new DateConverter();

export class DateTimeConverter implements IConverter {
  convertToSource(val: string, param: string): Date {
    if (!isString(val)) {
      return val as any;
    }
    const res = strToDate(val, param);
    return res;
  }
  convertToTarget(val: Date, param: string): string {
    const res = dateToStr(val, param);
    return res;
  }
  toString() {
    return "DateTimeConverter";
  }
}
export const dateTimeConverter = new DateTimeConverter();

export class NumberConverter implements IConverter {
  convertToSource(val: any, param: any): number {
    if (isNt(val)) {
      return null;
    }
    if (!isString(val)) {
      return val as any;
    }
    const dp = decimalPoint;
    let prec = 4;
    let value = val.replace(thousandSep, "");
    value = value.replace(dp, ".");
    value = stripNonNumeric(value);
    if (value === "") {
      return null;
    }
    let num: number = null;
    switch (param) {
      case NUM_CONV.SmallInt:
        num = parseInt(value, 10);
        break;
      case NUM_CONV.Integer:
        num = parseInt(value, 10);
        break;
      case NUM_CONV.Decimal:
        prec = decPrecision;
        num = round(parseFloat(value), prec);
        break;
      case NUM_CONV.Float:
        num = parseFloat(value);
        break;
      default:
        num = Number(value);
        break;
    }

    if (!isNumber(num)) {
      throw new Error("Invalid number" + num);
    }
    return num;
  }
  convertToTarget(val: any, param: any): string {
        if (isNt(val)) {
            return "";
        }
        const dp = decimalPoint;
        let prec: number;
        switch (param) {
            case NUM_CONV.Integer:
                prec = 0;
                return formatNumber(val, prec, dp, thousandSep);
            case NUM_CONV.Decimal:
                prec = decPrecision;
                return formatNumber(val, prec, dp, thousandSep);
            case NUM_CONV.SmallInt:
                prec = 0;
                return formatNumber(val, prec, dp, "");
            case NUM_CONV.Float:
                // float number type preserves all number precision
                return formatNumber(val, null, dp, thousandSep);
            default:
                return formatNumber(val, null, dp, thousandSep);
        }
    }
    toString() {
        return "NumberConverter";
    }
}
export const numberConverter = new NumberConverter();

export class IntegerConverter implements IConverter {
    convertToSource(val: any, _param: any): number {
        return numberConverter.convertToSource(val, NUM_CONV.Integer);
    }
    convertToTarget(val: any, _param: any): string {
        return numberConverter.convertToTarget(val, NUM_CONV.Integer);
    }
    toString() {
        return "IntegerConverter";
    }
}
export const integerConverter = new IntegerConverter();

export class SmallIntConverter implements IConverter {
    convertToSource(val: any, _param: any): number {
        return numberConverter.convertToSource(val, NUM_CONV.SmallInt);
    }
    convertToTarget(val: any, _param: any): string {
        return numberConverter.convertToTarget(val, NUM_CONV.SmallInt);
    }
    toString() {
        return "SmallIntConverter";
    }
}
export const smallIntConverter = new SmallIntConverter();

export class DecimalConverter implements IConverter {
    convertToSource(val: any, _param: any): number {
        return numberConverter.convertToSource(val, NUM_CONV.Decimal);
    }
    convertToTarget(val: any, _param: any): string {
        return numberConverter.convertToTarget(val, NUM_CONV.Decimal);
    }
    toString() {
        return "DecimalConverter";
    }
}
export const decimalConverter = new DecimalConverter();

export class FloatConverter implements IConverter {
    convertToSource(val: any, _param: any): number {
        return numberConverter.convertToSource(val, NUM_CONV.Float);
    }
    convertToTarget(val: any, _param: any): string {
        return numberConverter.convertToTarget(val, NUM_CONV.Float);
    }
    toString() {
        return "FloatConverter";
    }
}
export const floatConverter = new FloatConverter();


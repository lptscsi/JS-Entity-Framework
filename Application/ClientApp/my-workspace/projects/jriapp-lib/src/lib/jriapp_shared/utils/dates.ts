/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { StringUtils } from "./strutils";
import { Checks } from "./checks";
import { ERRS } from "../../lang";

const { isNt } = Checks, { format: formatStr } = StringUtils;

export enum TIME_KIND {
    YEAR = "year",
    MONTH = "month",
    WEEK = "week",
    DAY = "day",
    HOUR = "hour",
    MINUTE = "minute",
    SECOND = "second"
}

export type TIME_RANGE = TIME_KIND.YEAR | TIME_KIND.MONTH | TIME_KIND.WEEK | TIME_KIND.DAY;

function strToDate(val: string, format: string = "YYYYMMDD"): Date {
    if (!val) {
        return null;
    }

    const d = dayjs(val, format, true);

    if (!d.isValid()) {
        throw new Error(formatStr(ERRS.ERR_CONV_INVALID_DATE, val));
    }
    return d.toDate();
}

function dateToStr(dt: Date, format: string = "YYYYMMDD"): string {
    if (isNt(dt)) {
        return "";
    }
    return dayjs(dt).format(format);
}

function add(dt: Date, val: number, period: TIME_KIND): Date {
    return dayjs(dt).add(val, period).toDate();
}

export class DateUtils {
    static isValid(val: string, format: string = "YYYYMMDD"): boolean {
        if (!val) {
            return false;
        }

        const d = dayjs(val, format, true);
        return d.isValid();
    }
    static readonly strToDate: (val: string, format?: string) => Date = strToDate;
    static strToDatePartial(format?: string) {
        return (val: string) => strToDate(val, format);
    }
    static readonly dateToStr: (dt: Date, format?: string) => string = dateToStr;
    static dateToStrPartial(format?: string) {
        return (dt: Date) => dateToStr(dt, format);
    }
    static readonly add: (dt: Date, val: number, period: TIME_KIND) => Date = add;
    static addPartial1(period: TIME_KIND) {
        return (dt: Date, val: number) => add(dt, val, period);
    }
    static addPartial2(period: TIME_KIND) {
        return (val: number) => (dt: Date) => add(dt, val, period);
    }
    static addPartial3(period: TIME_KIND) {
        return (dt: Date) => (val: number) => add(dt, val, period);
    }
    static trim(dt: Date): Date {
        return dayjs(dt).startOf(TIME_KIND.DAY).toDate();
    }
    static today(): Date {
        return dayjs().startOf(TIME_KIND.DAY).toDate();
    }
    static now(): Date {
        return new Date();
    }
    static yesterday(dt?: Date): Date {
        return dayjs(dt).startOf(TIME_KIND.DAY).add(-1, TIME_KIND.DAY).toDate();
    }
    static tomorrow(dt?: Date): Date {
        return dayjs(dt).startOf(TIME_KIND.DAY).add(1, TIME_KIND.DAY).toDate();
    }
    static startOf(period: TIME_RANGE, dt?: Date): Date {
        return dayjs(dt).startOf(period).toDate();
    }
    static endOf(period: TIME_RANGE, dt?: Date): Date {
        return dayjs(dt).endOf(period).toDate();
    }
}

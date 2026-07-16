/** The MIT License (MIT) Copyright(c) 2016-present Maxim V.Tsapov */
import { IIndexer } from "../int";
import { ERROR } from "./error";
import { CoreUtils } from "./coreutils";


const { Indexer } = CoreUtils, error = ERROR, MAX_NUM = 99999900000;
let taskQueue: TaskQueue = null;

export interface IQueue {
    cancel: (taskId: number) => void;
    enque: (func: () => any) => number;
}

export interface ITaskQueue {
    enque(task: () => void): number;
    cancel(taskId: number): void;
}

interface ITask {
    taskId: number;
    func: () => void;
}

export function createQueue(interval: number = 0): IQueue {
    let _tasks: ITask[] = [], _taskMap: IIndexer<ITask> = Indexer(),
        _timer: number = null, _newTaskId = 1;

    const _queue: IQueue = {
        cancel: function (taskId: number): void {
            const task = _taskMap[taskId];
            if (!!task) {
                // cancel task by setting its func to null!!!
                task.func = null;
            }
        },
        enque: function (func: () => any): number {
            const taskId = _newTaskId;
            _newTaskId += 1;
            const task: ITask = { taskId: taskId, func: func };
            _tasks.push(task);
            _taskMap[taskId] = task;

            if (!_timer) {
                _timer = setTimeout(() => {
                    const arr = _tasks;
                    _timer = null;
                    _tasks = [];
                    // recycle generated nums if they are too big
                    if (_newTaskId > MAX_NUM) {
                        _newTaskId = 1;
                    }

                    try {
                        arr.forEach((task) => {
                            try {
                                if (!!task.func) {
                                    task.func();
                                }
                            } catch (err) {
                                error.handleError(_queue, err, _queue);
                            }
                        });
                    } finally {
                        // reset the map after all the tasks in the queue have been executed
                        // so a task can be cancelled from another task
                        _taskMap = Indexer();
                        // add tasks which were queued while tasks were executing (from inside the tasks) to the map
                        for (let i = 0; i < _tasks.length; i += 1) {
                            _taskMap[_tasks[i].taskId] = _tasks[i];
                        };
                    }
                }, interval);
            }

            return taskId;
        }
    };

    return _queue;
}


export function getTaskQueue(): ITaskQueue {
    if (!taskQueue) {
        taskQueue = new TaskQueue();
    }
    return taskQueue;
}

class TaskQueue implements ITaskQueue {
    private _queue: IQueue;

    constructor() {
        this._queue = createQueue(0);
    }

    enque(task: () => void): number {
        return this._queue.enque(task);
    }

    cancel(taskId: number): void {
        this._queue.cancel(taskId);
    }
}


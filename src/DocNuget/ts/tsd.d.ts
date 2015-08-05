declare module 'handlebars' {
  export function registerHelper(name: string, helper: Function): void
  export class SafeString {
    constructor(value: string)
  }
}

declare module 'fetch' {
  export interface Response {
    json(): Promise<any>
  }

  export function fetch(url: string): Promise<Response>

  export default fetch
}

declare var require: {
  <T>(path: string): T;
  (paths: string[], callback: (...modules: any[]) => void): void;
  ensure: (paths: string[], callback: (require: <T>(path: string) => T) => void) => void;
};

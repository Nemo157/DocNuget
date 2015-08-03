function join(context: any[], sep: string, options: any) {
  if (context) {
    return context
      .map(item => options.fn(item).trim())
      .join(sep)
  }
}

export default join

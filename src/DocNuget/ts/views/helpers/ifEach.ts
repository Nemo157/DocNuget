function ifEach(items: any[], title: string, options: any) {
  if (items && items.length) {
    return '<h4>' + title + '</h4>' + items.map(options.fn).join('\n')
  } else {
    return options.inverse(this)
  }
}

export default ifEach

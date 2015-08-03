function replace(str: string, substr: string, newSubStr: string) {
  if (str) {
    return str.replace(new RegExp(substr, 'g'), newSubStr)
  }
}

export default replace

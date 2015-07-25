function resolve (json: any) {
  let byid: {
    [key: string]: any
  } = {}
  let refs: Array<{
      parent: any
      prop: string
      ref: string
  }> = []; // references to objects that could not be resolved

  function recurse(obj: any, prop?: string, parent?: any) {
    if (obj && typeof obj === 'object') {
      if ("$ref" in obj) {
        if (obj.$ref in byid) {
          obj = byid[obj.$ref]
        } else {
          refs.push({ parent, prop, ref: obj.$ref })
        }
      } else if ("$id" in obj) {
        let id = obj.$id
        delete obj.$id

        if ("$values" in obj) {
          obj = obj.$values.map(recurse)
        } else {
          for (let prop in obj) {
            obj[prop] = recurse(obj[prop], prop, obj)
          }
        }

        byid[id] = obj
      }
    }

    return obj
  }

  recurse(json)

  refs.forEach(({ parent, prop, ref }, i) => parent[prop] = byid[ref])

  return json
}

export default resolve

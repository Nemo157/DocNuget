export var Accessibility: {
  [key: string]: number
  none: number
  public: number
  protected: number
  internal: number
  private: number
  all: number
} = {
  none: 0,
  public: 1,
  protected: 2,
  internal: 4,
  private: 8,
  all: 15,
}

Accessibility['<unknown>'] = Accessibility.all
Accessibility['protected internal'] = 6

export var settings = {
  accessibility: Accessibility.public | Accessibility.protected,
  accessibilityDebug: false,
}

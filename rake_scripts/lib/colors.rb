class String
  def colorize(color_code)
    "\e[#{color_code}m#{self}\e[0m"
  end

  def black
    colorize('30')
  end

  def dark_red
    colorize('31')
  end

  def dark_green
    colorize('32')
  end

  def dark_yellow
    colorize('33')
  end

  def dark_blue
    colorize('34')
  end

  def dark_magenta
    colorize('35')
  end

  def dark_cyan
    colorize('36')
  end

  def light_gray
    colorize('37')
  end

  def dark_gray
    colorize('30;1')
  end

  def red
    colorize('31;1')
  end

  def green
    colorize('32;1')
  end

  def yellow
    colorize('33;1')
  end

  def blue
    colorize('34;1')
  end

  def magenta
    colorize('35;1')
  end

  def cyan
    colorize('36;1')
  end

  def white
    colorize('37;1')
  end
end
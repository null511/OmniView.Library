class Object
  def blank?
    respond_to?(:empty?) ? !!empty? : !self
  end

  def to_bool?
    self.to_s == "true"
  end
end

class String
  def blank?
    self !~ /\S/ 
  end

  # munch.rb
  def ToSafeString
    self.strip.gsub(/'/, "''")
  end
end
require 'rake'
require 'pathname'

require_relative 'rake_scripts/lib/extensions.rb'

# Add all Ruby files in /rake_scripts subdirectory
Dir["./rake_scripts/*.rb"].each {|file| require_relative file }

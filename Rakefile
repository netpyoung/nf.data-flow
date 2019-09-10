require 'fileutils'

ROOT = Dir.pwd
BUILD_DIR = "#{ROOT}/__BUILD"

INPUT_EXCEL_FPATH = "#{ROOT}/exels/sample.xlsx"
TEMPLATE_DIR      = "#{ROOT}/solution/AutoGenerated.DB/template"
OUTPUT_CS_DIR     = "#{ROOT}/solution/AutoGenerated.DB/output"
GEN_DLL_FPATH     = "#{ROOT}/solution/AutoGenerated.DB/bin/Debug/netcoreapp2.0/AutoGenerated.DB.dll"
GEN_DB_FPATH      = "#{BUILD_DIR}/DB.db"
PASSWORD          = "helloworld"


# for macOs - net4.6
#ENV['FrameworkPathOverride'] = '/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.6-api/'
# DOTNET_RUN32 = 'dotnet mono -f net46 -mo="--arch=32 --debug" --loggerlevel Verbose'
# DOTNET_RUN64 = 'dotnet mono -f net462  -mo="--arch=64 --debug"'

task :default do
  sh 'rake -T'
end

desc 'install dotnet-nf-dataflow'
task :tool_install do
  Dir.chdir('solution') do
    Dir.chdir('dotnet-nf-dataflow') do
      sh 'dotnet pack'
      sh 'dotnet tool install --global --add-source ./nupkg dotnet-nf-dataflow'
    end
  end
end

desc 'uninstall dotnet-nf-dataflow'
task :tool_uninstall do
  sh 'dotnet tool uninstall -g dotnet-nf-dataflow'
end


desc("generate db & dll")
task :dataflow do
  FileUtils::mkdir_p BUILD_DIR
  Dir.chdir('solution') do
    sh "dotnet nf-dataflow code-gen -e #{INPUT_EXCEL_FPATH} -t #{TEMPLATE_DIR} -o #{OUTPUT_CS_DIR}"

    Dir.chdir('AutoGenerated.DB') do
      sh "dotnet build"
    end

    sh "dotnet nf-dataflow data-export -d #{GEN_DLL_FPATH} -e #{INPUT_EXCEL_FPATH} -o #{GEN_DB_FPATH} -p #{PASSWORD}"
  end

  cp_r "#{ROOT}/solution/AutoGenerated.DB/bin/Debug/netcoreapp2.0/", BUILD_DIR, :preserve => true
end

desc "update unityproject"
task :update_unityproject => [:default]  do
  output = 'unity_project/Assets/output'
  mkdir_p output

  # copy db
  db_files = Dir.glob("#{BUILD_DIR}/*.db")
  db_files.each do |db|
    cp_r(db, output)
  end

  # copy autogen
  cs_files = Dir.glob("solution/AutoGenerated.*/**/{Library.cs,*.autogen.cs}")
  cs_files.each do |cs|
    dir = "#{output}/#{File.dirname(cs)}"
    mkdir_p(dir)
    puts cs
    cp_r(cs, dir)
  end
end

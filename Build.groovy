pipeline {
	agent {
		label 'master'
	}

	stages {
		stage('Build') {
			steps {
				script {
					bat """
						nuget restore
						CALL bin\\msbuild_where.cmd \"OmniView.Library.sln\" /p:Configuration=Release /p:Platform=\"Any CPU\" /m
					"""

					//...
				}
			}
		}
	}
}

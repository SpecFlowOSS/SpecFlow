<!-- http://community.thoughtworks.com/posts/3967b9b234 -->
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:gallio="http://www.gallio.org/"
	version="1.0">

    <xsl:output method="xml" indent="yes" cdata-section-elements="message stack-trace message"/>

    <xsl:template match="/gallio:report">
		<test-results>
			
			<xsl:attribute name="name"><xsl:value-of select="gallio:testPackage/gallio:files/gallio:file"/></xsl:attribute>
			<xsl:attribute name="total"><xsl:value-of select="gallio:testPackageRun/gallio:statistics/@runCount"/></xsl:attribute>
			<xsl:attribute name="failures"><xsl:value-of select="gallio:testPackageRun/gallio:statistics/@failedCount"/></xsl:attribute>
			<xsl:attribute name="not-run"><xsl:value-of select="gallio:testPackageRun/gallio:statistics/@skippedCount"/></xsl:attribute>
			<xsl:attribute name="date"><xsl:value-of select="substring-before(gallio:testPackageRun/@startTime, 'T')"/></xsl:attribute>
			<xsl:attribute name="time"><xsl:value-of select="substring-before(substring-after(gallio:testPackageRun/@startTime, 'T'), '.')"/></xsl:attribute>
			
			<environment nunit-version="" clr-version="" os-version="" platform="" cwd="" machine-name="" user="" user-domain="" />
				
			<culture-info current-culture="en-US" current-uiculture="en-US" />
		
			<xsl:apply-templates select="gallio:testPackageRun/gallio:testStepRun/gallio:children" />
			
		</test-results>
    </xsl:template>

	<xsl:template match="gallio:testPackageRun/gallio:testStepRun/gallio:children">

		<xsl:apply-templates select="gallio:testStepRun" />

	</xsl:template>
	
	<xsl:template match="gallio:testStepRun">
		
		<xsl:choose>
			<xsl:when test="gallio:testStep/gallio:metadata/gallio:entry[@key='TestKind']/gallio:value = 'Test'">
				
				<test-case>
				
					<xsl:attribute name="name"><xsl:value-of select="gallio:testStep/gallio:codeReference/@type"/>.<xsl:value-of select="gallio:testStep/gallio:codeReference/@member"/></xsl:attribute>
					<xsl:attribute name="asserts"><xsl:value-of select="gallio:result/@assertCount"/></xsl:attribute>
					<xsl:attribute name="time"><xsl:value-of select="gallio:result/@duration"/></xsl:attribute>
					<xsl:attribute name="success"><xsl:choose><xsl:when test="gallio:result/gallio:outcome/@status = 'failed'">False</xsl:when><xsl:otherwise>True</xsl:otherwise></xsl:choose></xsl:attribute>
					<xsl:attribute name="executed"><xsl:choose><xsl:when test="gallio:result/gallio:outcome/@status = 'skipped'">False</xsl:when><xsl:otherwise>True</xsl:otherwise></xsl:choose></xsl:attribute>
					
					<xsl:if test="gallio:result/gallio:outcome/@status = 'failed'">
						<failure>
							  <message><xsl:value-of select="gallio:testLog/gallio:streams/gallio:stream[@name='Failures']/gallio:body/gallio:contents/gallio:section[@name='Message']/gallio:contents/gallio:text"/></message>
							  <stack-trace><xsl:value-of select="gallio:testLog/gallio:streams/gallio:stream[@name='Failures']/gallio:body/gallio:contents/gallio:section[@name='Stack Trace']/gallio:contents/gallio:marker[@class='StackTrace']/gallio:contents/gallio:text"/></stack-trace>
						</failure>
					</xsl:if>

					<xsl:if test="gallio:result/gallio:outcome/@status = 'skipped'">
						<reason>
						  <message><xsl:value-of select="gallio:testStep/gallio:metadata/gallio:entry[@key='IgnoreReason']/gallio:value"/></message>
						</reason>
					</xsl:if>
					
				</test-case>				
			
			</xsl:when>
			<xsl:otherwise>
			
				<test-suite>
					<xsl:attribute name="name"><xsl:choose><xsl:when test="string-length(../../gallio:testStep/@fullName) &gt; 0"><xsl:value-of select="substring-after(gallio:testStep/@fullName, concat(../../gallio:testStep/@fullName, '/'))"/></xsl:when><xsl:otherwise><xsl:value-of select="gallio:testStep/@fullName"/></xsl:otherwise></xsl:choose></xsl:attribute>
					<xsl:attribute name="asserts"><xsl:value-of select="gallio:result/@assertCount"/></xsl:attribute>
					<xsl:attribute name="time"><xsl:value-of select="gallio:result/@duration"/></xsl:attribute>
					<xsl:attribute name="success"><xsl:choose><xsl:when test="gallio:result/gallio:outcome/@status = 'failed'">False</xsl:when><xsl:otherwise>True</xsl:otherwise></xsl:choose></xsl:attribute>
							
					<xsl:apply-templates select="gallio:children" />
					
				</test-suite>
			
			</xsl:otherwise>
		</xsl:choose>	
	
	</xsl:template>
	
	<xsl:template match="gallio:children">

		<results>
		
			<xsl:apply-templates select="gallio:testStepRun" />
		
		</results>

	</xsl:template>
	
</xsl:stylesheet>

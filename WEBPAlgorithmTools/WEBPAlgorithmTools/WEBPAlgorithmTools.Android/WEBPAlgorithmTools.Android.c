#include "WEBPAlgorithmTools.h"
/*
* classe per chiamata ad algoritmo WebP Android
*/
	int WEBPEncode(const char * imageSource, const char * imageDestination, const char *quality) {
		return WEBPEncode_internal(imageSource, imageDestination, quality);
	}

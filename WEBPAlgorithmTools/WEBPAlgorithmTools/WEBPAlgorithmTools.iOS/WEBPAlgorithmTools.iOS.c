#include "WEBPAlgorithmTools.h"
/*
* classe per chiamata ad algoritmo WebP iOS
*/
int WEBPEncode(const char * imageSource, const char * imageDestination, const char *quality) {
	return WEBPEncode_internal(imageSource, imageDestination, quality);
}
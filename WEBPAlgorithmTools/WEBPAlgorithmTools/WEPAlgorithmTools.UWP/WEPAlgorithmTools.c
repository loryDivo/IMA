#include "WEBPAlgorithmTools.h"
/*
* classe per chiamata ad algoritmo WebP UWP
*/
__declspec(dllexport)int WEBPEncode(char * imageSource, char * imageDestination, const char *quality) {
	return WEBPEncode_internal(imageSource, imageDestination, quality);
}
add_executable(ShutterService.App
    MQTTc.cpp
    ClassPath/src/Address.cpp
    ClassPath/src/bstrwrap.cpp
    ClassPath/src/HashKey.cpp
    ClassPath/src/Lock.cpp
    ClassPath/src/Logger.cpp
    ClassPath/src/Strings.cpp
    ClassPath/src/Time.cpp
    ClassPath/src/Socket.cpp
    ClassPath/src/LinuxSpecific.cpp
    ClassPath/src/Dump.cpp

    ClassPath/src/bstrlib.c)


set(THREADS_PREFER_PTHREAD_FLAG ON)
find_package(Threads REQUIRED)

set_target_properties(ShutterService.App PROPERTIES
    CXX_STANDARD 11
    CXX_STANDARD_REQUIRED YES
    CXX_EXTENSIONS NO
)

include_directories(ClassPath/include/)

target_compile_definitions(ShutterService.App PUBLIC _DEBUG=$<CONFIG:Debug>
                                        MinimalFootPrint=$<STREQUAL:${REDUCED_FOOTPRINT},ON>
                                        MQTTOnlyBSDSocket=$<STREQUAL:${CROSSPLATFORM_SOCKET},OFF>
                                        MQTTUseTLS=$<AND:$<STREQUAL:${CROSSPLATFORM_SOCKET},OFF>,$<STREQUAL:${ENABLE_TLS},ON>>)


IF(CMAKE_BUILD_TYPE STREQUAL "MinSizeRel")
  IF (WIN32)
  ELSE()
    target_compile_options(ShutterService.App PUBLIC -fno-rtti -ffunction-sections -fdata-sections)
    target_link_options(ShutterService.App PUBLIC $<$<CXX_COMPILER_ID:GNU>:--gc-sections,--as-needed>)
    target_link_options(ShutterService.App PUBLIC $<$<CXX_COMPILER_ID:Clang>:-dead_strip>)
    target_link_options(ShutterService.App PUBLIC $<$<CXX_COMPILER_ID:AppleClang>:-dead_strip>)
  ENDIF()
ENDIF()


IF (CROSSPLATFORM_SOCKET STREQUAL OFF AND ENABLE_TLS STREQUAL ON)
    target_link_libraries(ShutterService.App LINK_PRIVATE mbedtls mbedcrypto mbedx509)
ENDIF()


target_link_libraries(ShutterService.App LINK_PUBLIC eMQTT5 ${CMAKE_DL_LIBS} Threads::Threads)
install(TARGETS ShutterService.App RUNTIME DESTINATION bin)


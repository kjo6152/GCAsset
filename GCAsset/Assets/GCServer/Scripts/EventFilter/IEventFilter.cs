using UnityEngine;
using System.Collections;

/**
 * @breif 필터 인터페이스
 * @details 여러 필터가 구현해야할 인터페이스
 * @see EventManager.receiveEvent
 * @author jiwon
 */
public interface IEventFilter {
    void filter(ref float[] data);
}
